using UnityEngine;
using System;
using UnityEngine.Events;    // UnityEvent



    public class ThankYouGesture : GestureRecognizerBase
    {

        [Header("손 오브젝트 참조")]
        public Transform leftPalm;
        public Transform rightPalm;
        public Transform leftBack;

        [Header("임계값")]
        public float contactDistance = 0.05f;
        public float contactSpeed = 0.3f;
        public float hitTimeout = 1f;        // 첫 타격 후 다음 타격 허용 시간(초)

        // 내부 상태
        bool poseOK = false;
        bool hit1OK = false;
        bool hit2OK = false;
        float hit1Time = 0f;
        Vector3 prevRightPos;

        [Header("손 준비 이벤트")]
        public HandTypeEvent OnHandReady;      // HandType을 파라미터로 받는 이벤트
        public HandTypeEvent OnHandCancelled;  // 취소용 이벤트
        [Header("두 번째 타격 최소 대기 시간")]
        [Tooltip("첫 타격 후 두 번째 타격을 인식하기 전 최소 대기 시간(초)")]
        public float minSecondInterval = 0.5f;

        protected override void Recognize()
        {
            // 1) 양손 준비 완료 시 포즈 타이머 시작
            if (!poseOK && handReady[0] && handReady[1])
            {
                poseOK = true;
                startTime = Time.time;
                prevRightPos = rightPalm.position;
                hit1Time = 0f;   // 첫 타격 전이므로 초기화
            }
            if (!poseOK) return;

            // 1.5) 첫 타격 후 타임아웃 체크
            if (hit1OK && !hit2OK && Time.time - hit1Time > hitTimeout)
            {
                // 두 번째 타격이 지정 시간 내에 없으면 첫 타격만 리셋
                hit1OK = false;
                Debug.Log("두 번째 타격 시간 초과, 첫 타격 리셋");
            }

            // 2) 타격 감지
            TryDetectHit();

            // 3) 두 번 타격 완료 시 제스처 인식
            if (hit1OK && hit2OK)
                CompleteGesture();
        }

        void TryDetectHit()
        {
            Vector3 cur   = rightPalm.position;
            float   dist  = Vector3.Distance(cur, leftBack.position);
            float   speed = Vector3.Distance(cur, prevRightPos) / Time.deltaTime;
            prevRightPos  = cur;

            // 1) 첫 번째 타격 체크
            if (!hit1OK)
            {
                if (dist < contactDistance && speed > contactSpeed)
                {
                    hit1OK   = true;
                    hit1Time = Time.time;
                    Debug.Log("첫 타격 OK");
                }
                return; // 두 번째 타격 검사로 넘어가지 않음
            }

            // 2) 첫 타격이 이미 OK이고, 두 번째 타격이 아직 OK가 아닐 때만 검사
            if (!hit2OK)
            {
                // 첫 타격 후 너무 짧으면 감지하지 않음
                if (Time.time - hit1Time < minSecondInterval)
                    return;

                if (dist < contactDistance && speed > contactSpeed)
                {
                    hit2OK = true;
                    Debug.Log("두 번째 타격 OK");
                }
            }
        }

        protected override void ResetGesture()
        {
            Debug.Log("reset");
            base.ResetGesture();
            poseOK = false;
            hit1OK = false;
            hit2OK = false;
            hit1Time = 0f;
            // 손 준비 플래그도 함께 리셋하고 싶다면 아래 주석 해제
            // handReady[0] = handReady[1] = false;
        }

        /// <summary>
        /// 지정된 손이 준비(포즈 OK) 상태임을 외부에서 호출
        /// </summary>
    }
