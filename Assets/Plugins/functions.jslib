mergeInto(LibraryManager.library, {
  CommitToYandex: function (data) {
    if (!player) {
      return;
    }

    const dataString = UTF8ToString(data);
    const dataObject = JSON.parse(dataString);
    player.setData(dataObject);
  },
  FetchFromYandex: function () {
    if (!player) {
      return;
    }

    player.getData().then((data) => {
      const jsonData = JSON.stringify(data);

      myGameInstance.SendMessage(
        "UserDataController",
        "SetUserDataFromJson",
        jsonData
      );
    });
  },
});
