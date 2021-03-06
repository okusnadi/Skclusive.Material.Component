﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Skclusive.Core.Component;
using Skclusive.Script.DomHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skclusive.Material.Button
{
    public class ButtonBaseComponent : ButtonCommonComponent
    {
        public ButtonBaseComponent() : base("ButtonBase")
        {
        }

        [Inject]
        public DomHelpers DomHelpers { set; get; }

        protected Skclusive.Core.Component.Component Control { set; get; }

        protected ElementReference? ButtonRef { get => Control.RootRef.Current; }

        protected TouchRipple Ripple { set; get; }

        [Parameter]
        public ButtonType Type { set; get; } = ButtonType.Button;

        [Parameter]
        public bool CenterRipple { set; get; }

        [Parameter]
        public bool FocusRipple { set; get; }

        [Parameter]
        public bool? FocusVisible { set; get; }

        protected bool LastDisabled { set; get; }

        protected string _Type => _Component == "button" ? Type.ToString().ToLower() : null;

        protected string _Component
        {
            get => string.IsNullOrWhiteSpace(Component) ? !string.IsNullOrWhiteSpace(Href) ? "a" : "button" : Component;
        }

        // public override string Role
        // {
        //     get => !string.Equals(_Component, "button") ? "button" : null;
        // }

        protected override IEnumerable<string> Classes
        {
            get
            {
                foreach (var item in base.Classes)
                    yield return item;

                if (FocusVisible.HasValue && FocusVisible.Value)
                {
                    yield return nameof(FocusVisible);

                    yield return $"~{FocusVisibleClass}";
                }
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (!FocusVisible.HasValue || (FocusVisible.Value && (Disabled.HasValue && Disabled.Value)))
            {
                FocusVisible = false;
            }
        }

        public void MakeFocusVisible()
        {
            FocusVisible = true;

            Control?.Focus();
        }

        //public override Task SetParametersAsync(ParameterView parameters)
        //{
        //    var disabled = parameters.GetValueOrDefault(nameof(Disabled), false);

        //    if (!FocusVisible.HasValue || (FocusVisible.Value && disabled))
        //    {
        //        FocusVisible = false;
        //        LastDisabled = disabled;
        //    }
        //    else
        //    {
        //        LastDisabled = disabled;
        //    }

        //    return base.SetParametersAsync(parameters);
        //}

        protected override void HandleFocus(FocusEventArgs args)
        {
            if (!(Disabled.HasValue && Disabled.Value))
            {
                base.HandleFocus(args);
            }
        }

        protected override async Task HandleBlurAsync(FocusEventArgs args)
        {
            FocusVisible = false;

            await base.HandleBlurAsync(args);

            Ripple?.Stop();
        }

        protected override async Task HandleMouseUpAsync(MouseEventArgs args)
        {
            await base.HandleMouseUpAsync(args);

            Ripple?.Stop();
        }

        protected override async Task HandleMouseDownAsync(MouseEventArgs args)
        {
            var point = new TouchPoint
            {
                IsTouch = false,

                ClientX = args.ClientX,

                ClientY = args.ClientY
            };

            var boundry = await DomHelpers.GetBoundryAsync(RootRef.Current);

            Ripple?.Start(FocusVisible, CenterRipple, point, boundry);

            await base.HandleMouseDownAsync(args);
        }

        protected override async Task HandleDragLeaveAsync(DragEventArgs args)
        {
            await base.HandleDragLeaveAsync(args);

            Ripple?.Stop();
        }

        protected override async Task HandleMouseLeaveAsync(EventArgs args)
        {
            await base.HandleMouseLeaveAsync(args);

            Ripple?.Stop();
        }

        /* TODO: OnTouchStart=HandleTouchStartAsync not used to avoid double ripple. Need to revist.  */
        protected override async Task HandleTouchStartAsync(TouchEventArgs args)
        {
            var point = new TouchPoint
            {
                IsTouch = true,

                ClientX = args.Touches[0].ClientX,

                ClientY = args.Touches[0].ClientY
            };

            var boundry = await DomHelpers.GetBoundryAsync(RootRef.Current);

            Ripple?.Start(FocusVisible, CenterRipple, point, boundry);

            await base.HandleTouchStartAsync(args);
        }

        protected override async Task HandleTouchEndAsync(TouchEventArgs args)
        {
            await base.HandleTouchEndAsync(args);

            Ripple?.Stop();
        }

        protected override async Task HandleTouchMoveAsync(TouchEventArgs args)
        {
            await base.HandleTouchMoveAsync(args);

            Ripple?.Stop();
        }

        // TODO: Need to do keydown and keyup

        private static TouchPoint PULSATE_POINT = new TouchPoint
        {
            IsTouch = false,

            ClientX = 0,

            ClientY = 0
        };

        private static Boundry PULSATE_BOUNDRY = new Boundry
        {
            Top = 0,

            Left = 0,

            Width = 0,

            Height = 0
        };

        protected override void OnAfterRender()
        {
            base.OnAfterRender();

            if(FocusVisible.HasValue && FocusVisible.Value)
            {
                Ripple?.Pulsate(PULSATE_POINT, PULSATE_BOUNDRY);
            }
        }

        protected override void OnAfterMount()
        {
            base.OnAfterMount();
        }

        protected override void OnAfterUpdate()
        {
            base.OnAfterUpdate();
        }

        protected override void OnAfterUnmount()
        {
            base.OnAfterUnmount();
        }
    }

    public enum ButtonType
    {
        Submit,

        Reset,

        Button
    }
}
