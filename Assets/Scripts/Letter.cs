using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Donutask.Wordfall
{
    public class Letter : MonoBehaviour
    {
        public char letter { get; private set; }
        public static readonly float stepTime = 0.175f;
        static readonly int stepsPerGravity = 5;
        protected SpriteRenderer spriteRenderer;

        int stepCount;
        bool locked;

        private void Start()
        {
            StartCoroutine(Step());
            locked = false;
        }

        private ParticleSystem shineParticles;
        public void SetLetter(char l)
        {
            letter = l;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = WordManager.GetLetterSprite(l);

            //Blank tiles have special particles (becuase they are special)
            if (letter == WordManager.blank)
            {
                if (shineParticles == null)
                {
                    shineParticles = LetterParticlesManager.Instance.CreateAndPlayParticles(transform, ParticleType.BlankShine);
                }
                else
                {
                    shineParticles.Play();
                }
            }
            else
            {
                if (shineParticles != null)
                    shineParticles.Stop();
            }
        }

        IEnumerator Step()
        {
            while (!locked)
            {
                yield return new WaitForSeconds(stepTime);
                //Don't run if paused
                if (PauseManager.paused || BlankLetterChooser.choosingLetter)
                {
                    continue;
                }
                //Definately stop moving if placed
                if (locked)
                {
                    yield break;
                }

                stepCount++;

                //Falling
                if ((stepCount % stepsPerGravity == 0 && stepCount > 0) || ControlsManager.IsDropping())
                {
                    if (transform.position.y >= Grid.height && Grid.DoesLetterExistInDirection(this, Vector2Int.down))
                    {
                        //If too high up, end the game, unless it is a bomb.
                        //If you lose to a bomb being out of the grid it feels kinda unfair
                        if (letter == WordManager.bomb)
                        {
                            PlaceLetter();
                        }
                        else
                        {
                            GameOver.Instance.EndGame();
                            yield break;
                        }
                    }
                    transform.position += Vector3.down;
                }
                else
                {
                    Vector2 movement = ControlsManager.GetLetterMovement();

                    //Prevent moving into another letter (this check doesn't run if above the grid)
                    if (transform.position.y >= Grid.height || !Grid.DoesLetterExistInDirection(this, new Vector2Int((int)movement.x, 0)))
                    {
                        //Apply but clamp to fit in board
                        transform.position += (Vector3)movement;
                        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, Grid.width - 1), transform.position.y, 0);
                    }
                }

                //If at bottom or thing underneath, place letter
                if (transform.position.y <= 0 || Grid.DoesLetterExistInDirection(this, Vector2Int.down))
                {
                    if (transform.position.y >= Grid.height)
                    {
                        //If too high up, end the game, unless it is a bomb.
                        //If you lose to a bomb being out of the grid it feels kinda unfair
                        if (letter == WordManager.bomb)
                        {
                            PlaceLetter();
                        }
                        else
                        {
                            GameOver.Instance.EndGame();
                            yield break;
                        }
                    }
                    else
                    {
                        PlaceLetter();
                    }
                }
            }
        }

        void PlaceLetter()
        {
            if (locked == false)
            {
                locked = true;

                if (letter == WordManager.blank)
                {
                    //Blanks need to choose the letter, then we lock it in
                    BlankLetterChooser.ChooseBlank((char l) =>
                    {
                        SetLetter(l);
                        WordChecker.LockInLetter(this);
                        var particleBurst = LetterParticlesManager.Instance.CreateAndPlayParticles(transform, ParticleType.BlankChosen);
                        Destroy(shineParticles.gameObject, 1f);
                        Destroy(particleBurst.gameObject, 1f);
                    });
                }
                else
                {
                    WordChecker.LockInLetter(this);
                }

                LetterSpawner.spawnTile.Invoke();

                OnPlace();
            }
        }

        void OnPlace()
        {
            //Explode
            if (letter == WordManager.bomb)
            {
                LetterParticlesManager.Instance.CreateAndPlayParticles(transform, ParticleType.Bomb);
                Destroy(gameObject, 0.9f);

                spriteRenderer.enabled = false;
            }
            else if (letter == WordManager.blank)
            {
                //blank choosing happens above
            }
            //Just place
            else
            {
                AudioManager.instance.Play("Place");
            }

            //Shine not needed after place (if not blank) (give 1 sec for particles to naturally fade out)
            if (letter != WordManager.blank)
                if (shineParticles != null)
                {
                    Destroy(shineParticles.gameObject, 1f);
                }
        }

        /// <summary>
        /// Removes from grid and destroys object
        /// </summary>
        public void Clear()
        {
            Grid.UnassignLetter(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Removes from grid, moves transform down, re-assigns to grid (so internal representation is updated to match visuals)
        /// </summary>
        public void Fall()
        {
            Grid.UnassignLetter(this);
            transform.position += Vector3.down;
            Grid.AssignLetter(this);
        }
    }
}