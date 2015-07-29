function redirectToGlyma(listId, itemId) {
    var ctx = new SP.ClientContext.get_current();
    this.web = ctx.get_web();
    this.properties = web.get_allProperties();
    var lists = web.get_lists();
    var list = lists.getById(listId);
    this.item = list.getItemById(itemId);
    this.file = item.get_file();
    ctx.load(this.web);
    ctx.load(this.properties);
    ctx.load(this.item);
    ctx.load(this.file);
    ctx.executeQueryAsync(Function.createDelegate(this, itemUrlLoadSuccessful), Function.createDelegate(this, itemUrlLoadFailed));
}

function itemUrlLoadSuccessful() {
    var propertyBag = this.properties.get_fieldValues();
    var defaultGlymaPage = propertyBag["glyma.defaultpage"];
    var videoSource = this.file.get_serverRelativeUrl();
    videoSource = window.location.protocol + "//" + window.location.host + videoSource;
    var defaultGlymaUrl = window.location.protocol + "//" + window.location.host + defaultGlymaPage + "?VideoSource=" + videoSource;
    window.location.href = defaultGlymaUrl;
}

function itemUrlLoadFailed() {
    var failureMessage = "The request failed.";
}