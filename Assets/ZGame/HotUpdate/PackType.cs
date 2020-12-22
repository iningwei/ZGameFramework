using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.HotUpdate
{
    /// <summary>
    /// 打包类型
    /// </summary>
    public enum PackType
    {
        /// <summary>
        /// 开发
        /// </summary>
        DEV,

        /// <summary>
        /// 发布
        /// </summary>
        PUB,

        /// <summary>
        /// 审核（直接以游客的方式连接外网）
        /// </summary>
        AUDIT,

        /// <summary>
        /// 测试
        /// </summary>
        TEST,
    }
}