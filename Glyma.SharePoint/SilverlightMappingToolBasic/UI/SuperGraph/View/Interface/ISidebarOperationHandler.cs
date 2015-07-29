namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface ISidebarOperationHandler : IOperationHandler, IZoomControl
    {
        
        void HorizontalRealign(bool isPartialRealign = false);
        void VerticalRealign(bool isPartialRealign = false);

        void ReaderMode();
        void AuthorMode();


    }
}
