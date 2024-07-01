using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameState.Timer
{
    /// <summary>
    /// 간단한 타이머 기능을 하는 커스텀 클래스
    /// </summary>
    public class GameTimer : MonoBehaviour
    {

        private float playingTime;
        private CancellationTokenSource token;
        public event Action<float> OnPlayingTimeChangedEvent;
        public float GetPlayingTime => playingTime;

        public void Reset()
        {
            Stop();
            playingTime = 0f;
        }
        public void Restart()
        {
            token = new();
            RunTick(token).Forget();
        }
        public void Stop()
        {
            token?.Cancel();
        }

        public void Begin()
        {
            Reset();
            Restart();
        }

        private async UniTaskVoid RunTick(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.NextFrame();
                playingTime += Time.deltaTime;
                OnPlayingTimeChangedEvent?.Invoke(playingTime);
            }
        }

    }

}
