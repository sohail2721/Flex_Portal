var db;
var request = indexedDB.open("MyDatabase");

request.onerror = function (event) {
    console.log("Error opening database");
};

request.onupgradeneeded = function (event) {
    var db = event.target.result;
    var objectStore = db.createObjectStore("files", { keyPath: "id" });
};

request.onsuccess = function (event) {
    db = event.target.result;
};

window.writeToFile = function (filename, content)  {
    var transaction = db.transaction(["files"], "readwrite");
    var objectStore = transaction.objectStore("files");
    var file = { id: filename, content: content };
    var request = objectStore.put(file);
    request.onerror = function (event) {
        console.log("Error writing file to database");
    };
}

