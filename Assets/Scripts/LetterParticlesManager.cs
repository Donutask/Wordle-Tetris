using UnityEngine;

namespace Donutask.Wordfall
{
    public class LetterParticlesManager : MonoBehaviour
    {
        public ParticleSystem bombParticles, blankShineParticles, blankChosenParticles;
        public static LetterParticlesManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public ParticleSystem CreateAndPlayParticles(Transform parent, ParticleType type)
        {
            GameObject obj;
            switch (type)
            {
                case ParticleType.Bomb:
                    obj = bombParticles.gameObject;
                    break;
                case ParticleType.BlankShine:
                    obj = blankShineParticles.gameObject;
                    break;
                case ParticleType.BlankChosen:
                    obj = blankChosenParticles.gameObject;
                    break;
                default:
                    obj = null;
                    break;
            }
            GameObject clone = GameObject.Instantiate(obj, parent);
            var particles = clone.GetComponent<ParticleSystem>();
            particles.Play();

            return particles;
        }


    }

    public enum ParticleType
    {
        Bomb,
        BlankShine,
        BlankChosen,
    }
}