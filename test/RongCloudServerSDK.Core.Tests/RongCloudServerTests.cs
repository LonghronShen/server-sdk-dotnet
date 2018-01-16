using System;
using Xunit;
using RongCloudServerSDK;
using donet.io.rong;
using io.rong;
using donet.io.rong.messages;

namespace RongCloudServerSDK.Tests
{

    public class RongCloudServerTests
    {

        public const string appKey = "82hegw5uhqtcx";
        public const string appSecret = "FnngEX4S3aB";

        [Fact]
        public async void GetTokenAsyncTest()
        {
            var retstr = await RongCloudServer.GetTokenAsync(appKey, appSecret, "232424", "xugang", "http://www.qqw21.com/article/UploadPic/2012-11/201211259378560.jpg");
            Assert.False(string.IsNullOrEmpty(retstr));
        }

        [Fact]
        public async void SyncGroupAsyncTest()
        {
            string[] arrId = { "group001", "group002", "group003" };
            string[] arrName = { "测试 01", "测试 02", "测试 03" };
            var retstr = await RongCloudServer.SyncGroupAsync(appKey, appSecret, "42424", arrId, arrName);
            Assert.True(retstr);
        }

        [Fact]
        public async void DismissGroupAsyncTest()
        {
            var retstr = await RongCloudServer.DismissGroupAsync(appKey, appSecret, "42424", "group001");
            Assert.True(retstr);
        }

        [Fact]
        public async void PublishMessageAsyncTest()
        {
            var retstr = await RongCloudServer.PublishMessageAsync(appKey, appSecret, "d9314722-3649-48c7-abef-6c90edcf444c",
                new string[] { "d9314722-3649-48c7-abef-6c90edcf444c", "d9314722-3649-48c7-abef-6c90edcf777c" }, new TxtMessage("c#hello", null), msgtype: false);
            Assert.True(retstr);
        }

        [Fact]
        public async void BroadcastMessageAsyncTest()
        {
            var retstr = await RongCloudServer.BroadcastMessageAsync(appKey, appSecret, "2191", new TxtMessage("c#hello", null));
            Assert.True(retstr);
        }

        [Fact]
        public async void JoinGroupAsyncTest()
        {
            var retstr = await RongCloudServer.JoinGroupAsync(appKey, appSecret, new[] { "423424" }, "dwef", "dwef");
            Assert.True(retstr);
        }

        [Fact]
        public async void CreateChatroomAsyncTest()
        {
            string[] arrId = { "group001", "group002", "group003" };
            string[] arrName = { "测试 01", "测试 02", "测试 03" };
            var retstr = await RongCloudServer.CreateChatroomAsync(appKey, appSecret, arrId, arrName);
            Assert.True(retstr);
        }

        [Fact]
        public async void DestroyChatroomAsyncTest()
        {
            var retstr = await RongCloudServer.DestroyChatroomAsync(appKey, appSecret, new string[] { "001", "002" });
            Assert.True(retstr);
        }

        [Fact]
        public async void QueryChatroomAsyncTest()
        {
            string[] arrGroups = { "group002", "group003" };
            var retstr = await RongCloudServer.QueryChatroomAsync(appKey, appSecret, arrGroups);
            Assert.True(retstr != null);
        }

        [Fact]
        public async void RefreshUserInfoAsyncTest()
        {
            var userId = "d9314722-3649-48c7-abef-6c90edcf444c";
            var name = "test";
            var portraitUri = "";
            var retstr = await RongCloudServer.RefreshUserInfoAsync(appKey, appSecret, userId, name, portraitUri);
            Assert.True(retstr);
        }

    }

}