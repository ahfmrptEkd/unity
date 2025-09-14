using UnityEngine;

namespace UnityAlgorithms.Unity
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Source")]
        public AudioSource sfxSource;    // 효과음용
        
        [Header("Sound Effects")]
        public AudioClip discDropSound;   // 토큰 떨어지는 소리
        
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float sfxVolume = 0.7f;    // 효과음 볼륨
        
        // 싱글톤 패턴
        public static AudioManager Instance;
        
        void Awake()
        {
            // 싱글톤 패턴 구현
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // 효과음만 사용하므로 배경음악은 자동 재생하지 않음
        }
        
        // AudioSource 초기화
        private void InitializeAudioSources()
        {
            // AudioSource가 없으면 자동으로 추가
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f; // 2D 사운드
            }
            
            // 볼륨 설정
            UpdateVolumes();
        }
        
        // 볼륨 설정 업데이트
        private void UpdateVolumes()
        {
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }
        
        // Inspector에서 볼륨 변경 시 실시간 반영
        void OnValidate()
        {
            UpdateVolumes();
        }
        
        // === 효과음 재생 메서드 ===
        
        public void PlayDiscDrop()
        {
            PlaySFX(discDropSound);
        }
        
        // 범용 효과음 재생 메서드
        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else if (clip == null)
            {
                Debug.LogWarning("AudioManager: AudioClip is null");
            }
        }
        
        
        // === 볼륨 조절 메서드 ===
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }
        
        // 사운드 음소거/해제
        public void MuteAll(bool mute)
        {
            if (sfxSource != null) sfxSource.mute = mute;
        }
    }
}