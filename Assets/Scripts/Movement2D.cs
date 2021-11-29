using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 0.2f;
    private bool isMove = false;

    public bool MoveTo(Vector3 moveDirection)
    {
        //이동중이면 이동함수 실행하지 않도록 함
        if(isMove)
        {
            return false;
        }
        //현재 위치로부터 1만큼 이동한 위치를 매개변수로 받는 코루틴함수 실행
        StartCoroutine(SmoothGridMovement(transform.position + moveDirection));
        return true;
    }

    private IEnumerator SmoothGridMovement(Vector2 endPosition)
    {
        Vector2 startPosition = transform.position;
        float percent = 0;

        isMove = true;

        while (percent < moveTime)
        {
            percent += Time.deltaTime;

            transform.position = Vector2.Lerp(startPosition, endPosition, percent / moveTime);

            yield return null;
        }
        isMove = false;

    }


}
