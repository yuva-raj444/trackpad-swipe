using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Input;
using Windows.UI.Core;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace TrackpadSwipeApp
{
    public sealed partial class InvisibleWindow : Window
    {
        private GestureService _gestureService;

        public InvisibleWindow()
        {
            this.InitializeComponent();

            _gestureService = new GestureService(DispatcherQueue.GetForCurrentThread());

            this.PointerPressed += OnPointerPressed;
            this.PointerMoved += OnPointerMoved;
            this.PointerReleased += OnPointerReleased;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _gestureService.ProcessPointerEvent(e.GetCurrentPoint(this));
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _gestureService.ProcessPointerEvent(e.GetCurrentPoint(this));
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _gestureService.ProcessPointerEvent(e.GetCurrentPoint(this));
        }
    }
}