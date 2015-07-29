using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using SilverlightMappingToolBasic.MappingService;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Windows.Messaging;

using SilverlightMappingToolBasic.Controls;


namespace SilverlightMappingToolBasic.SingleDepth
{
    public partial class CompendiumFirstDepthMap : UserControl, IMapControl
    {
        private TypeManager _typeManager;
        private SingleDepthNavigator _navigator;
        private DatabaseMappingService _nodeService;
        public event EventHandler NavigatorInitialised;

        public string DomainUid
        {
            get;
            set;
        }

        public string NodeUid
        {
            get;
            set;
        }

        public LocalMessageSender MessageSender
        {
            get;
            set;
        }

        public EventHandler<MessageReceivedEventArgs> MessageReceivedHandler
        {
            get;
            set;
        }

        public INodeNavigator Navigator
        {
            get
            {
                return _navigator;
            }
        }

        public Canvas MapSurface
        {
            get
            {
                return FindName("uxMapSurface") as Canvas;
            }
        }

        public INodeProxy[] SelectedNodes
        {
            get
            {
                List<INodeProxy> selectedNodes = new List<INodeProxy>();
                if (_navigator != null)
                {
                    selectedNodes.AddRange(_navigator.SelectedNodes);
                }
                return selectedNodes.ToArray();
            }
        }

        public CompendiumFirstDepthMap()
        {
            InitializeComponent();

            if (!IsInDesignMode)
            {
                _nodeService = new DatabaseMappingService();

                _typeManager = new TypeManager(_nodeService);
                IoC.IoCContainer.GetInjectionInstance().RegisterComponent<TypeManager>(_typeManager);

                _typeManager.InitialiseNodeTypeManagerCompleted += new EventHandler(InitialiseNodeTypeManagerCompleted);
                _typeManager.InitialiseNodeTypeManager();
            }
        }

        private void InitialiseNodeTypeManagerCompleted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DomainUid))
            {
                Guid domainId = Guid.Empty;
                if (Guid.TryParse(DomainUid, out domainId))
                {
                    if (!string.IsNullOrEmpty(NodeUid))
                    {
                        Guid nodeId = Guid.Empty;
                        if (Guid.TryParse(NodeUid, out nodeId))
                        {
                            InitializeNavigator(nodeId, domainId);
                        }
                    }

                    else
                    {
                        _nodeService.GetDomainNodeIdCompleted += new EventHandler<ReturnedNodeIdEventArgs>(OnGetDomainNodeIdCompleted);
                        _nodeService.GetDomainNodeIdAsync(domainId);
                    }
                }
            }
        }

        private void OnGetDomainNodeIdCompleted(object sender, ReturnedNodeIdEventArgs e)
        {
            if (!string.IsNullOrEmpty(DomainUid) && e.NodeId != null)
            {
                Guid domainId = Guid.Empty;
                if (Guid.TryParse(DomainUid, out domainId))
                {
                    InitializeNavigator(e.NodeId, domainId);
                }
            }
        }

        private void InitializeNavigator(Guid nodeId, Guid domainId)
        {
            ThemeManager themeManagementObject = IoC.IoCContainer.GetInjectionInstance().GetInstance<ThemeManager>();
            themeManagementObject.LoadTheme("Generic");

            _navigator = new SingleDepthNavigator(_nodeService, themeManagementObject, domainId);
            IoC.IoCContainer.GetInjectionInstance().RegisterComponent<SingleDepthNavigator>(_navigator);
            if (NavigatorInitialised != null)
            {
                NavigatorInitialised.Invoke(this, new EventArgs());
            }
            _navigator.SetCurrentNode(nodeId);
            _navigator.GetCurrentNodesCompleted += new EventHandler<RendererNodesEventArgs>(OnGetCurrentNodesCompleted);
            _navigator.GetCurrentNodesAsync();
        }

        private void OnGetCurrentNodesCompleted(object sender, RendererNodesEventArgs e)
        {
            // TODO: When TODO item ID:1001 is being done a replacement for the following needs to be considered too.
            this.uxMapSurface.Children.Clear();

            RenderingContextInfo contextInfo = new RenderingContextInfo();

            contextInfo.SurfaceHeight = this.Height;
            contextInfo.SurfaceWidth = this.Width;
            contextInfo.SurfaceTopLeftX = 0;
            contextInfo.SurfaceTopLeftY = 0;

            foreach (KeyValuePair<Guid, INodeRenderer> nodeRenderer in e.View.NodeRenderers)
            {
                nodeRenderer.Value.Context = contextInfo;
                UIElement nodeRendererControl = nodeRenderer.Value as UIElement;

                if (nodeRendererControl != null)
                {
                    this.uxMapSurface.Children.Add(nodeRendererControl);
                }
            }

            foreach (KeyValuePair<Guid, IRelationshipRenderer> relationshipRenderer in e.View.RelationshipRenderers)
            {
                UIElement relationshipRendererControl = relationshipRenderer.Value as UIElement;

                if (relationshipRendererControl != null)
                {
                    this.uxMapSurface.Children.Add(relationshipRendererControl);
                }
            }
        }

        #region Design Mode Helpers
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running under Blend
        /// or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }
        #endregion

    }
}
