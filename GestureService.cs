using Microsoft.UI.Input;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.System;
using System;
using System.Runtime.InteropServices;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using Microsoft.UI.Dispatching;

namespace TrackpadSwipeApp
{
    public class GestureService
    {
        private GestureRecognizer _gestureRecognizer;
        private InputSimulator _inputSimulator;
        private DispatcherQueue _dispatcherQueue;

        public GestureService(DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;
            _inputSimulator = new InputSimulator();
            _gestureRecognizer = new GestureRecognizer();
            _gestureRecognizer.GestureSettings = GestureSettings.ManipulationTranslateX;

            _gestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
        }

        public void ProcessPointerEvent(PointerPoint pointerPoint)
        {
            _gestureRecognizer.ProcessDownEvent(pointerPoint);
            _gestureRecognizer.ProcessMoveEvents(new[] { pointerPoint });
            _gestureRecognizer.ProcessUpEvent(pointerPoint);
        }

        private void GestureRecognizer_ManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            var delta = args.Delta.Translation.X;

            if (Math.Abs(delta) < 50)
                return;

            if (delta > 0)
            {
                SendAltLeft();
            }
            else if (delta < 0)
            {
                SendAltRight();
            }
        }

        private void SendAltLeft()
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.LEFT);
            });
        }

        private void SendAltRight()
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                _inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.RIGHT);
            });
        }
    }
}