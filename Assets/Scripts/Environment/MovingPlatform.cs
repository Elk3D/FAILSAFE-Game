using System.Collections;
using UnityEngine;

namespace FAILSAFE.Environment
{
    /// <summary>
    /// A platform that moves between two waypoints on a loop.
    /// Players standing on it are parented temporarily for smooth riding.
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Waypoints")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;

        [Header("Movement")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float waitTime = 1f;

        private bool _movingToB = true;

        private void Start() => StartCoroutine(Move());

        private IEnumerator Move()
        {
            while (true)
            {
                var target = _movingToB ? pointB.position : pointA.position;
                while (Vector3.Distance(transform.position, target) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                    yield return null;
                }
                transform.position = target;
                yield return new WaitForSeconds(waitTime);
                _movingToB = !_movingToB;
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.CompareTag(Utilities.Constants.TAG_PLAYER))
                col.transform.SetParent(transform);
        }

        private void OnCollisionExit(Collision col)
        {
            if (col.gameObject.CompareTag(Utilities.Constants.TAG_PLAYER))
                col.transform.SetParent(null);
        }
    }
}
