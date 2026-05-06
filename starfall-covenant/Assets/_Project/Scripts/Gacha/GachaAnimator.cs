using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Gacha
{
    public class GachaAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject pullEffectPrefab;
        [SerializeField] private GameObject revealEffectPrefab;
        [SerializeField] private Transform revealPoint;
        [SerializeField] private float pullAnimDuration = 2f;
        [SerializeField] private float revealDelay = 0.5f;

        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor = Color.gray;
        [SerializeField] private Color rareColor = Color.blue;
        [SerializeField] private Color epicColor = new Color(0.6f, 0f, 0.8f);
        [SerializeField] private Color legendaryColor = Color.yellow;
        [SerializeField] private Color mythicColor = new Color(1f, 0.4f, 0f);

        public System.Action<GachaPullResult> OnRevealComplete;

        public void PlayPullAnimation(GachaPullResult result)
        {
            StartCoroutine(PullSequence(result));
        }

        public void PlayMultiPullAnimation(List<GachaPullResult> results)
        {
            StartCoroutine(MultiPullSequence(results));
        }

        private IEnumerator PullSequence(GachaPullResult result)
        {
            if (pullEffectPrefab != null)
            {
                Instantiate(pullEffectPrefab, revealPoint.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(pullAnimDuration);

            if (revealEffectPrefab != null)
            {
                var reveal = Instantiate(revealEffectPrefab, revealPoint.position, Quaternion.identity);
                var particles = reveal.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    var main = particles.main;
                    main.startColor = GetRarityColor(result.rarity);
                }
            }

            yield return new WaitForSeconds(revealDelay);
            OnRevealComplete?.Invoke(result);
        }

        private IEnumerator MultiPullSequence(List<GachaPullResult> results)
        {
            if (pullEffectPrefab != null)
            {
                Instantiate(pullEffectPrefab, revealPoint.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(pullAnimDuration);

            foreach (var result in results)
            {
                if (revealEffectPrefab != null)
                {
                    var reveal = Instantiate(revealEffectPrefab, revealPoint.position, Quaternion.identity);
                    var particles = reveal.GetComponent<ParticleSystem>();
                    if (particles != null)
                    {
                        var main = particles.main;
                        main.startColor = GetRarityColor(result.rarity);
                    }
                }

                yield return new WaitForSeconds(revealDelay);
                OnRevealComplete?.Invoke(result);
                yield return new WaitForSeconds(0.3f);
            }
        }

        private Color GetRarityColor(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common => commonColor,
                Rarity.Rare => rareColor,
                Rarity.Epic => epicColor,
                Rarity.Legendary => legendaryColor,
                Rarity.Mythic => mythicColor,
                _ => Color.white
            };
        }
    }
}
