mergeInto(LibraryManager.library, {
  CommitToYandex: function (data) {
    if (!player) {
      return;
    }

    const dataString = UTF8ToString(data);
    const dataObject = JSON.parse(dataString);
    player.setData(dataObject, true);
  },
  FetchFromYandex: function () {
    if (!player) {
      return;
    }

    player.getData([
      "<Money>k__BackingField",
      "<BestMetersRecord>k__BackingField",
      "<CharacterKeys>k__BackingField",
      "<SelectedCharacterKey>k__BackingField",
      "<LocalizationCode>k__BackingField"])
    .then((data) => {
      const jsonData = JSON.stringify(data);

      myGameInstance.SendMessage(
        "UserDataController",
        "SetUserDataFromJson",
        jsonData
      );
    });
  },
  SetLeaderboard: function () {
    ysdk.getLeaderboards().then((leaderboard) => {
      player.getData().then((data) => {
        leaderboard.setLeaderboardScore(
          "leaderBoardMain",
          data["<BestMetersRecord>k__BackingField"]
        );
      });
    });
  },
});
