using GDLibrary.Core;
using System;

namespace GDLibrary.Components.UI
{
    public class UIProgressBarController : UIController
    {
        #region Fields

        private int currentValue;
        private int maxValue;
        private int startValue;
        private UITextureObject parentUITextureObject;

        #endregion Fields

        #region Properties

        public int CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = ((value >= 0) && (value <= maxValue)) ? value : 0;
            }
        }

        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = (value >= 0) ? value : 0;
            }
        }

        public int StartValue
        {
            get
            {
                return startValue;
            }
            set
            {
                startValue = (value >= 0) ? value : 0;
            }
        }

        #endregion Properties

        public UIProgressBarController(int startValue, int maxValue)
        {
            StartValue = startValue;
            MaxValue = maxValue;
            CurrentValue = startValue;

            //listen for UI events to change the SourceRectangle
            EventDispatcher.Subscribe(EventCategoryType.UI, HandleEvents);
        }

        #region Event Handling

        private void HandleEvents(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnHealthDelta)
            {
                //get the name of the ui object targeted by this event
                var targetUIObjectName = eventData.Parameters[0] as string;

                //is it for me?
                if (targetUIObjectName != null
                    && uiObject.Name.Equals(targetUIObjectName))
                    CurrentValue = currentValue + (int)eventData.Parameters[1];
            }
        }

        public override void Update()
        {
            //TODO - wasteful, called each update - refactor
            //update draw source rectangle based on current value
            UpdateSourceRectangle();
        }

        protected void UpdateSourceRectangle()
        {
            //try to cast the parent that this component is attached to
            parentUITextureObject = uiObject as UITextureObject;

            if (parentUITextureObject == null)
                return;

            //how much of a percentage of the width of the image does the current value represent?
            var widthMultiplier = (float)currentValue / maxValue;

            //now set the amount of visible rectangle using the current value
            parentUITextureObject.SourceRectangleWidth = (int)Math.Round(widthMultiplier * parentUITextureObject.OriginalSourceRectangle.Width);
        }

        #endregion Event Handling

        #region Actions - Input

        protected override void HandleInputs()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleKeyboardInput()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleMouseInput()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleGamepadInput()
        {
            throw new System.NotImplementedException();
        }

        #endregion Actions - Input

        //to do...Equals, GetHashCode, Clone
    }
}

/*
namespace GDLibrary.Components.UI
{
    public class UIProgressBarController : UIController
    {
        #region Fields

        private int currentValue;
        private int maxValue;
        private int startValue;
        private Rectangle sourceRectangle;

        #endregion Fields

        #region Properties

        public int CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = ((value >= 0) && (value <= maxValue)) ? value : 0;
            }
        }

        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = (value >= 0) ? value : 0;
            }
        }

        public int StartValue
        {
            get
            {
                return startValue;
            }
            set
            {
                startValue = (value >= 0) ? value : 0;
            }
        }

        public UIProgressBarController(int startValue,
            int maxValue, int currentValue)
        {
            StartValue = startValue;
            MaxValue = maxValue;
            CurrentValue = currentValue;
        }

        public override void Update()
        {
            if (uiObject != null)
            {
                (uiObject as UITextureObject).SetRectangle(
                    64 * CurrentValue / MaxValue, 8);
            }

            HandleInputs();
        }

        protected override void HandleInputs()
        {
            HandleKeyboardInput();
        }

        protected override void HandleKeyboardInput()
        {
            if (Input.Keys.WasJustPressed(Keys.Up))
            {
                //increment current value
                CurrentValue++;
            }
            else if (Input.Keys.WasJustPressed(Keys.Down))
            {
                //decrement current value
                CurrentValue--;
            }
        }

        protected override void HandleMouseInput()
        {
            throw new System.NotImplementedException();
        }

        protected override void HandleGamepadInput()
        {
            throw new System.NotImplementedException();
        }

        #endregion Properties

        //to do...Equals, GetHashCode, Clone
    }
}*/