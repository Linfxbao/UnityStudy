using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{

    // 重写权威判定，返回 false 即代表允许客户端权威同步
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
