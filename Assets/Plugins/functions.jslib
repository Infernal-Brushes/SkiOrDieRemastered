mergeInto(LibraryManager.library, {
  CommitToYandex: (data) => {
    const dataString = UTF8ToString(data);
    const dataObject = JSON.parse(dataString);
    player.setData(dataObject);
  },
  FetchFromYandex: () => {
    player.getData().then((data) => {
      const jsonData = JSON.stringify(data);
      myGameInstance.SendMessage("PlayerData: ", jsonData);
    });
  },
});
