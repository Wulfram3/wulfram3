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
<<<<<<< HEAD

mergeInto(LibraryManager.library, WebLocalStorage);
=======
mergeInto(LibraryManager.library, WebLocalStorage);

>>>>>>> ef5921745e1ba675bdf67d3d3d48c3d26a1c7cd1
