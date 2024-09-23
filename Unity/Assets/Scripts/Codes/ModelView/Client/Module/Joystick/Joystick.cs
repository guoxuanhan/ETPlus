using FairyGUI;
using UnityEngine;

namespace ET.Client
{
    public class Joystick: EventDispatcher
    {
        private float _initX;
        private float _initY;
        private float _startStageX;
        private float _startStageY;
        private float _lastStageX;
        private float _lastStageY;
        private float _lastDegrees;

        private GButton _button;
        private GObject _touchArea;
        private GObject _thumb;
        private GObject _center;
        private GTweener _tweener;

        private int _touchId;

        public EventListener onStart { get; private set; }
        public EventListener onMove { get; private set; }
        public EventListener onEnd { get; private set; }

        public int Radius { get; set; }

        public Joystick(GComponent mainView)
        {
            this.onStart = new EventListener(this, "onStart");
            this.onMove = new EventListener(this, "onMove");
            this.onEnd = new EventListener(this, "onEnd");

            this._button = mainView.GetChild("joystick").asButton;
            this._button.changeStateOnClick = false;

            this._thumb = this._button.GetChild("thumb");
            this._touchArea = mainView.GetChild("joystick_touch");
            this._center = mainView.GetChild("joystick_center");

            this._initX = this._center.x + this._center.width / 2;
            this._initY = this._center.y + this._center.height / 2;
            this._touchId = -1;
            this.Radius = 150;

            this._touchArea.onTouchBegin.Add(this.onTouchBegin);
            this._touchArea.onTouchMove.Add(this.onTouchMove);
            this._touchArea.onTouchEnd.Add(this.onTouchEnd);
        }

        public void Trigger(EventContext context)
        {
            onTouchBegin(context);
        }

        private void onTouchBegin(EventContext context)
        {
            if (this._touchId == -1)
            {
                InputEvent evt = (InputEvent)context.data;
                this._touchId = evt.touchId;

                if (this._tweener != null)
                {
                    this._tweener.Kill();
                    this._tweener = null;
                }

                Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
                float bx = pt.x;
                float by = pt.y;
                this._button.selected = true;

                if (bx < 0) bx = 0;
                else if (bx > this._touchArea.width) bx = this._touchArea.width;

                if (by > GRoot.inst.height) by = GRoot.inst.height;
                else if (by < this._touchArea.y) by = this._touchArea.y;

                this._lastStageX = bx;
                this._lastStageY = by;
                this._startStageX = bx;
                this._startStageY = by;

                this._center.visible = true;
                this._center.SetXY(bx - this._center.width / 2, by - this._center.height / 2);
                this._button.SetXY(bx - this._button.width / 2, by - this._button.height / 2);

                float deltaX = bx - this._initX;
                float deltaY = by - this._initY;
                this._lastDegrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
                this._thumb.rotation = this._lastDegrees + 90;

                context.CaptureTouch();
                this.onStart.Call(this._lastDegrees);
            }
        }

        private void onTouchMove(EventContext context)
        {
            InputEvent evt = (InputEvent)context.data;
            if (this._touchId != -1 && evt.touchId == this._touchId)
            {
                Vector2 pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
                float bx = pt.x;
                float by = pt.y;
                float moveX = bx - this._lastStageX;
                float moveY = by - this._lastStageY;
                this._lastStageX = bx;
                this._lastStageY = by;
                float buttonX = this._button.x + moveX;
                float buttonY = this._button.y + moveY;

                float offsetX = buttonX + this._button.width / 2 - this._startStageX;
                float offsetY = buttonY + this._button.height / 2 - this._startStageY;

                float rad = Mathf.Atan2(offsetY, offsetX);
                this._lastDegrees = rad * 180 / Mathf.PI;
                _thumb.rotation = this._lastDegrees + 90;

                float maxX = this.Radius * Mathf.Cos(rad);
                float maxY = this.Radius * Mathf.Sin(rad);
                if (Mathf.Abs(offsetX) > Mathf.Abs(maxX)) offsetX = maxX;
                if (Mathf.Abs(offsetY) > Mathf.Abs(maxY)) offsetY = maxY;

                buttonX = this._startStageX + offsetX;
                buttonY = this._startStageY + offsetY;
                if (buttonX < 0) buttonX = 0;
                if (buttonY > GRoot.inst.height) buttonY = GRoot.inst.height;

                this._button.SetXY(buttonX - this._button.width / 2, buttonY - this._button.height / 2);
                this.onMove.Call(this._lastDegrees);
            }
        }

        private void onTouchEnd(EventContext context)
        {
            InputEvent inputEvt = (InputEvent)context.data;
            if (this._touchId != -1 && inputEvt.touchId == this._touchId)
            {
                this._touchId = -1;
                this._thumb.rotation = this._thumb.rotation + 180;
                this._center.visible = false;
                this._tweener = _button.TweenMove(new Vector2(this._initX - this._button.width / 2, this._initY - this._button.height / 2), 0.3f)
                        .OnComplete(() =>
                        {
                            this._tweener = null;
                            this._button.selected = false;
                            this._thumb.rotation = 0;
                            this._center.visible = true;
                            this._center.SetXY(this._initX - this._center.width / 2, this._initY - this._center.height / 2);
                        });

                this.onEnd.Call(this._lastDegrees);
            }
        }
    }
}