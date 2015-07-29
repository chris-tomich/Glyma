using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace SilverlightMappingToolBasic
{
    public class MouseClickManager
    {
        private event MouseButtonEventHandler _click;
        private event MouseButtonEventHandler _doubleClick;
        private event MouseButtonEventHandler _heldClick;

        public event MouseButtonEventHandler Click
        {
            add { _click += value; }
            remove { _click -= value; }
        }

        public event MouseButtonEventHandler DoubleClick
        {
            add { _doubleClick += value; }
            remove { _doubleClick -= value; }
        }

        public event MouseButtonEventHandler HeldClick
        {
            add { _heldClick += value; }
            remove { _heldClick -= value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MouseClickManager"/> is clicked.
        /// </summary>
        /// <value><c>true</c> if clicked; otherwise, <c>false</c>.</value>
        private bool Clicked { get; set; }

        /// <summary>
        /// Gets or sets the timeout for how long between clicks a double click is.
        /// </summary>
        /// <value>The timeout.</value>
        public int DoubleClickTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout for how long a held click is.
        /// </summary>
        /// <value>The timeout.</value>
        public int HeldClickTimeout { get; set; }

        public FrameworkElement FrameworkElement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseClickManager"/> class.
        /// </summary>
        public MouseClickManager(FrameworkElement control, int doubleClickTimeout, int heldClickTimeout)
        {
            this.FrameworkElement = control;
            this.Clicked = false;
            this.DoubleClickTimeout = doubleClickTimeout;
            this.HeldClickTimeout = heldClickTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseClickManager"/> class with default values
        /// Double Click Timeout = 400ms
        /// Held Click Timeout = 2000ms.
        /// </summary>
        public MouseClickManager(FrameworkElement frameworkElement)
            : this(frameworkElement, 400, 1000)
        {
        }

        /// <summary>
        /// Handles the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                if (this.Clicked)
                {
                    //Debug.WriteLine("HandleMouseUp: was clicked, resetting");
                    this.Clicked = false;
                    OnDoubleClick(sender, e);
                }
                else
                {
                    //Debug.WriteLine("HandleMouseUp: not clicked, starting double click timer");
                    this.Clicked = true;
                    ParameterizedThreadStart doubleClickThreadStart = new ParameterizedThreadStart(ResetDoubleClickThread);
                    Thread doubleClickThread = new Thread(doubleClickThreadStart);
                    doubleClickThread.Start(e);
                }
            }
        }

        private Point _mouseDownLocation;

        public void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                //Debug.WriteLine("HandleMouseDown: setting to clicked");

                if (sender is SilverlightMappingToolBasic.MapDepth.CompendiumMapDepthMap)
                {
                    SilverlightMappingToolBasic.MapDepth.CompendiumMapDepthMap mapSurface = (SilverlightMappingToolBasic.MapDepth.CompendiumMapDepthMap)sender;
                    mapSurface.Focus();
                }

                this.Clicked = true;
                _mouseDownLocation = e.GetPosition(null);
                ParameterizedThreadStart heldClickThreadStart = new ParameterizedThreadStart(ResetHeldClickThread);
                Thread heldClickThread = new Thread(heldClickThreadStart);
                heldClickThread.Start(e);
            }
        }

        /// <summary>
        /// Resets the thread.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ResetDoubleClickThread(object state)
        {
            Thread.Sleep(this.DoubleClickTimeout);

            lock (this)
            {
                if (this.Clicked)
                {
                    //Debug.WriteLine("ResetDoubleClickThread: was clicked, resetting");
                    this.Clicked = false;
                    OnClick(this, (MouseButtonEventArgs)state);
                }
            }
        }

        private void ResetHeldClickThread(object state)
        {
            Thread.Sleep(this.HeldClickTimeout);
            MouseButtonEventArgs e = state as MouseButtonEventArgs;
            lock (this)
            {
                FrameworkElement.Dispatcher.BeginInvoke(delegate()
                {
                    Point newLocation = e.GetPosition(null);
                    if (newLocation.Equals(_mouseDownLocation))
                    {
                        if (this.Clicked)
                        {
                            //Debug.WriteLine("ResetHeldClickThread: was clicked, resetting");
                            this.Clicked = false;
                            OnHeldClick(this, e);
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("Mouse Moved");
                    }
                });
            }
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            if (_click != null)
            {
                (sender as Control).Dispatcher.BeginInvoke(_click, sender, e);
            }
        }

        /// <summary>
        /// Called when [double click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_doubleClick != null)
            {
                _doubleClick(sender, e);
            }
        }

        /// <summary>
        /// Called when the mouse is held down for a set period.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnHeldClick(object sender, MouseButtonEventArgs e)
        {
            if (_heldClick != null)
            {
                _heldClick(sender, e);
            }
        }
    }
}
