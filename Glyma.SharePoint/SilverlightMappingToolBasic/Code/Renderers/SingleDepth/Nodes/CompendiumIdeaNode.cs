using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IoC;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumIdeaNode : CompendiumGenericNode
    {
        public CompendiumIdeaNode()
        {
        }

        protected override Brush NodeImage
        {
            get
            {
                if (NodeSettings.IsFocused)
                {
                    return IoCContainer.GetInjectionInstance().GetInstance<FullIdeaResource>().SpriteResouce;
                }
                else
                {
                    return IoCContainer.GetInjectionInstance().GetInstance<CompactIdeaResource>().SpriteResouce;
                }
            }
        }

        #region ICompendiumNode Members

        public override void LoadSettings(CompendiumNodeSettings settings)
        {
            base.LoadSettings(settings);
        }

        public override UIElement[] RenderUIElements()
        {
            return base.RenderUIElements();
        }

        #endregion
    }
}
