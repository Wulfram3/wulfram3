var WebLocalStorage = {
    GetValue: function(key){
        var data = localStorage.getItem(Pointer_stringify(key));
		if(data === null){
			data = "null";
		}

		var buffer = _malloc(lengthBytesUTF8(data) + 1);
		stringToUTF8(data, buffer, data.length + 1);
        return buffer;
    },
    SetValue: function(key, value){
        localStorage.setItem(Pointer_stringify(key), Pointer_stringify(value));
    }
};
mergeInto(LibraryManager.library, WebLocalStorage);