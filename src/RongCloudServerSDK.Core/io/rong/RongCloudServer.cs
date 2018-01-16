using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using donet.io.rong;
using donet.io.rong.models;
using donet.io.rong.messages;
using Newtonsoft.Json;

#if NETCORE
using Mono.Web;
#else
using System.Web;
#endif

namespace io.rong
{

    public class RongCloudServer
    {

        public static bool DebugLog = false;

        public static async Task<bool> InvokeAsync(string appKey, string appSecret, Func<RongCloud, CodeSuccessReslut> invoker, bool treatAsException = false)
        {
            var result = await Task.Run(() => invoker(RongCloud.getInstance(appKey, appSecret)));
            if (DebugLog)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
            if (result == null || result.getCode() != 200)
            {
                if (treatAsException)
                {
                    throw new Exception(result?.getErrorMessage());
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static async Task<T> InvokeAsync<T>(string appKey, string appSecret, Func<RongCloud, T> invoker, bool treatAsException = false) where T : IRongMessageResult
        {
            var result = await Task.Run(() => invoker(RongCloud.getInstance(appKey, appSecret)));
            if (DebugLog)
            {
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
            if (result == null || result.getCode() != 200)
            {
                if (treatAsException)
                {
                    throw new Exception(result?.getErrorMessage());
                }
            }
            return result;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="portraitUri"></param>
        /// <returns></returns>
        public static async Task<string> GetTokenAsync(string appkey, string appSecret, string userId, string name, string portraitUri)
        {
            var client = RongCloud.getInstance(appkey, appSecret);
            var result = client.user.getToken(userId, name, portraitUri);
            if (result == null || result.getCode() != 200)
            {
                throw new Exception(result?.getErrorMessage());
            }
            return await Task.FromResult(result.getToken());
        }

        /// <summary>
        /// 刷新用户信息 方法
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId">用户 Id</param>
        /// <param name="name">用户名称</param>
        /// <param name="portraitUri">用户头像 URI</param>
        /// <returns></returns>
        public static async Task<bool> RefreshUserInfoAsync(string appkey, string appSecret, string userId, string name, string portraitUri)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                client.user.refresh(userId, name, portraitUri));
        }

        /// <summary>
        /// 加入群组
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static async Task<bool> JoinGroupAsync(string appkey, string appSecret, string[] userId, string groupId, string groupName)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                client.group.join(userId, groupId, groupName));
        }

        /// <summary>
        /// 退出群组
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static async Task<bool> QuitGroupAsync(string appkey, string appSecret, string[] userId, string groupId)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                client.group.quit(userId, groupId));
        }

        /// <summary>
        /// 解散群组
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static async Task<bool> DismissGroupAsync(string appkey, string appSecret, string userId, string groupId)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                 client.group.dismiss(userId, groupId));
        }

        /// <summary>
        /// 同步群组
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static async Task<bool> SyncGroupAsync(string appkey, string appSecret, string userId, string[] groupId, string[] groupName)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(nameof(groupId));
            }
            if (groupId.Length != groupName.Length)
            {
                throw new ArgumentOutOfRangeException($"{nameof(groupId)}.Length != {nameof(groupName)}.Length.");
            }
            var list = new List<GroupInfo>();
            for (int i = 0; i < groupId.Length; i++)
            {
                list.Add(new GroupInfo(groupId[i], groupName[i]));
            }
            return await InvokeAsync(appkey, appSecret, client =>
                 client.group.sync(userId, list.ToArray()));
        }

        /// <summary>
        /// 同步群组
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="userId"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static async Task<bool> SyncGroupAsync(string appkey, string appSecret, string userId, GroupInfo[] groups)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                 client.group.sync(userId, groups));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="fromUserId"></param>
        /// <param name="toUserIds"></param>
        /// <param name="content"></param>
        /// <param name="pushContent"></param>
        /// <param name="pushData"></param>
        /// <param name="count"></param>
        /// <param name="verifyBlacklist"></param>
        /// <param name="isIncludeSender"></param>
        /// <param name="isPersisted"></param>
        /// <param name="isCounted"></param>
        /// <param name="msgtype">单聊/系统消息 false 单聊 true 系统消息 默认 单聊</param>
        /// <returns></returns>
        public static async Task<bool> PublishMessageAsync(string appkey, string appSecret, string fromUserId, string[] toUserIds, IRongMessage content, string pushContent = null, string pushData = null, int? count = null, bool verifyBlacklist = false, bool isIncludeSender = false, bool isPersisted = false, bool isCounted = false, bool msgtype = false)
        {
            return await InvokeAsync(appkey, appSecret, client =>
            {
                if (msgtype)
                {
                    return client.message.publishSystem(fromUserId, toUserIds, content,
                        pushContent, pushData, isPersisted ? 1 : 0, isCounted ? 1 : 0);
                }
                else
                {
                    return client.message.publishPrivate(fromUserId, toUserIds, content,
                       pushContent, pushData, count,
                       verifyBlacklist ? 1 : 0,
                       isPersisted ? 1 : 0,
                       isCounted ? 1 : 0,
                       isIncludeSender ? 1 : 0);
                }
            });
        }

        /// <summary>
        /// 消息历史记录下载地址获取
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="date">指定北京时间某天某小时，格式为2014010101,表示：2014年1月1日凌晨1点。（必传）</param>
        /// <returns></returns>
        public static async Task<string> GetHistoryAsync(string appkey, string appSecret, string date)
        {
            var result = await InvokeAsync(appkey, appSecret, client =>
                 client.message.getHistory(date));
            return result?.getUrl();
        }

        /// <summary>
        /// 消息历史记录删除
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="date">指定北京时间某天某小时，格式为2014010101,表示：2014年1月1日凌晨1点。（必传）</param>
        /// <returns></returns>
        public static async Task<bool> DeleteHistoryAsync(string appkey, string appSecret, string date)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                client.message.deleteMessage(date));
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="fromUserId"></param>
        /// <param name="message"></param>
        /// <param name="pushContent"></param>
        /// <param name="pushData"></param>
        /// <param name="os"></param>
        /// <returns></returns>
        public static async Task<bool> BroadcastMessageAsync(string appkey, string appSecret, string fromUserId, IRongMessage message, string pushContent = null, string pushData = null, string os = null)
        {
            return await InvokeAsync(appkey, appSecret, client =>
                client.message.broadcast(fromUserId, message, pushContent, pushData, os));
        }

        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="chatroomInfo">chatroom[id10001]=name1001</param>
        /// <returns></returns>
        public static async Task<bool> CreateChatroomAsync(string appkey, string appSecret, string[] chatroomId, string[] chatroomName)
        {
            if (chatroomName == null)
            {
                throw new ArgumentNullException(nameof(chatroomName));
            }
            if (chatroomId.Length != chatroomName.Length)
            {
                throw new ArgumentOutOfRangeException($"{nameof(chatroomId)}.Length != {nameof(chatroomName)}.Length.");
            }
            var list = new List<ChatRoomInfo>();
            for (int i = 0; i < chatroomId.Length; i++)
            {
                list.Add(new ChatRoomInfo(chatroomId[i], chatroomName[i]));
            }
            return await InvokeAsync(appkey, appSecret, client =>
               client.chatroom.create(list.ToArray()));
        }

        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="chatRoomInfo"></param>
        /// <returns></returns>
        public static async Task<bool> CreateChatroomAsync(string appkey, string appSecret, ChatRoomInfo[] chatRoomInfo)
        {
            return await InvokeAsync(appkey, appSecret, client =>
               client.chatroom.create(chatRoomInfo));
        }

        /// <summary>
        /// 销毁聊天室
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="chatroomIdInfo">chatroomId=id1001</param>
        /// <returns></returns>
        public static async Task<bool> DestroyChatroomAsync(string appkey, string appSecret, string[] chatroomIdInfo)
        {
            return await InvokeAsync(appkey, appSecret, client =>
               client.chatroom.destroy(chatroomIdInfo));
        }

        /// <summary>
        /// 查询聊天室
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="appSecret"></param>
        /// <param name="chatroomId"></param>
        /// <returns></returns>
        public static async Task<List<ChatRoom>> QueryChatroomAsync(string appkey, string appSecret, string[] chatroomId)
        {
            var result = await InvokeAsync(appkey, appSecret, client =>
                client.chatroom.query(chatroomId));
            return result?.getChatRooms();
        }

    }

}