using UnityEngine;
using System.Collections;
using Com.Wulfram3;
    namespace OptimizedGuy
    {
        /// <summary>
        /// Example implementation of IEnemy
        /// </summary>
        public class Enemy : AbstractEnemy
        {
            public bool isDead = false;
            public float life = 100;
            public Hazard hazard = Hazard.EASY;
            public ParticleSystem deadExplosion;
            public bool showDebugInfo = true;
            public float baseVelocity = 1f;

            private Transform myTransform;

            // Use this for initialization
            void Start()
            {
                myTransform = transform;
            }

            // Update is called once per frame
            void Update()
            {
                myTransform.Translate(myTransform.right * baseVelocity * Time.deltaTime);
            }


            public override bool IsDead()
            {
                return isDead;
            }

            public override float GetLife()
            {
                return life;
            }

            public override int GetHazard()
            {
                return (int)hazard;
            }

            public void Damage(float damageQuantity)
            {
                life -= damageQuantity;

                if (life <= damageQuantity)
                {
                    isDead = true;

                    Explode();
                }
            }

            public void OnGUI()
            {
                if (showDebugInfo && GetComponent<Renderer>().isVisible)
                {
                    Vector3 viewPortPosition = Camera.main.WorldToScreenPoint(transform.position);

                    float x = viewPortPosition.x;
                    float y = Screen.height - viewPortPosition.y;

                    GUI.Box(new Rect(x, y, 125, 65), name);

                    GUI.TextField(new Rect(x + 2f, y + 20f, 120, 20), "Life:" + life);

                    if (hazard == Hazard.EASY)
                        GUI.color = Color.green;
                    else if (hazard == Hazard.NORMAL)
                        GUI.color = Color.yellow;
                    else if (hazard == Hazard.HARD)
                        GUI.color = Color.red;

                    GUI.TextField(new Rect(x + 2f, y + 40f, 120, 20), "Hazard:" + hazard);
                }
            }

            public void Explode()
            {
                deadExplosion = Instantiate(deadExplosion, transform.position, Quaternion.identity) as ParticleSystem;
                Destroy(deadExplosion, 1f);
                Destroy(gameObject);
            }

            public void OnCollisionEnter(Collision targetCollider)
            {
                ExampleBullet3D bullet = targetCollider.gameObject.GetComponent<ExampleBullet3D>();
            
            if (bullet != null)
                {
                //GetComponent<HitPointsManager>().TakeDamage(-1);
                Damage(bullet.bulletDamage);
                }
            }
        }
    }
