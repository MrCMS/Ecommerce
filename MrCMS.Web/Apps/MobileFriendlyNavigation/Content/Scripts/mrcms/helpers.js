(function() {
    if (!String.prototype.supplant) {
        String.prototype.supplant = function(o) {
            return this.replace(/{([^{}]*)}/g,
                function(a, b) {
                    var r = o[b];
                    return (typeof r === 'string' || typeof r === 'number' || typeof r === 'boolean') ? r : a;
                }
            );
        };
    }
}());