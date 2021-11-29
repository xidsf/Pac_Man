using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    [SerializeField]
    private Sprite[] images;
    [SerializeField]
    private StageData stageData;
    [SerializeField]
    private float delayTime = 3.0f;

    private LayerMask tileLayer;
    private float rayDistance = 0.55f;
    private Vector2 moveDirection = Vector2.right;
    private Direction direction = Direction.Right;
    private Direction nextDirection = Direction.None;

    private Movement2D movement2D;
    private AroundWrap aroundWrap;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        tileLayer = 1 << LayerMask.NameToLayer("Tile");
        movement2D = GetComponent<Movement2D>();
        aroundWrap = GetComponent<AroundWrap>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetMoveDirectionByRandom();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, rayDistance, tileLayer);
        if (hit.transform == null)
        {
            movement2D.MoveTo(moveDirection);
            aroundWrap.UpdateAroundWarp();
        }
        else
        {
            SetMoveDirectionByRandom();
        }
        
    }

    private void SetMoveDirection(Direction direction)
    {
        this.direction = direction;
        moveDirection = Vector3FromEnum(this.direction);
        spriteRenderer.sprite = images[(int)this.direction];

        StopAllCoroutines();
        StartCoroutine("SetMoveDirectionByTime");

    }

    private void SetMoveDirectionByRandom()
    {
        //�̵� ���� ���� ���� 
        direction = (Direction)Random.Range(0, (int)Direction.Count);
        SetMoveDirection(direction);
    }

    private IEnumerator SetMoveDirectionByTime()
    {
        yield return new WaitForSeconds(delayTime);

        //���� �̵� ������ Right or Left�̸� direction & 2 = 0����
        //���� �̵��淮(nextDirection)�� up(1) �Ǵ� Down(3)���� ����

        //���� �̵� ������ Up or Down�̸� direction & 2 = 1����
        //���� �̵��淮(nextDirection)�� Right(0) �Ǵ� Left(2)���� ����

        int dir = Random.Range(0, 2);
        nextDirection = (Direction)(dir * 2 + 1 - (int)direction % 2);
        StartCoroutine("CheckBlockedNextMoveDirection");
    }

    private IEnumerator CheckBlockedNextMoveDirection()
    {
        while(true)
        {
            Vector3[] directions = new Vector3[3];
            bool[] isPossibleMoves = new bool[3];

            directions[0] = Vector3FromEnum(nextDirection);

            //nextdirection������ ������, ������ ��쿡�� 
            if(directions[0].x != 0)
            {
                directions[1] = directions[0] + new Vector3(0, 0.65f, 0);
                directions[2] = directions[0] + new Vector3(0, -0.65f, 0);
            }
            else if (directions[0].y != 0)
            {
                directions[1] = directions[0] + new Vector3(-0.65f, 0, 0);
                directions[2] = directions[0] + new Vector3(0.65f, 0, 0);
            }
            //�̵� �������� Ȯ���ϱ� ���� ����3�� �߻�

            int possibleCount = 0 ;

            for (int i = 0; i < 3; i++)
            {
                if(i == 0)
                {
                    isPossibleMoves[i] = Physics2D.Raycast(transform.position, directions[i], 0.5f, tileLayer);
                    Debug.DrawLine(transform.position, transform.position + directions[i] * 0.5f, Color.yellow);
                }
                else 
                {
                    isPossibleMoves[i] = Physics2D.Raycast(transform.position, directions[i], 0.7f, tileLayer);
                    Debug.DrawLine(transform.position, transform.position + directions[i] * 0.7f, Color.yellow);
                }
                if(isPossibleMoves[i] == false)
                {
                    possibleCount++;
                }

            }
            //3���� ������ �ε����� ���� ���� ��
            if (possibleCount == 3)
            {
                //�ܰ����� ������ �� �̵��ϸ� �ȵǱ� ������ �������� ���� �ִ��� �˻�
                if(transform.position.x > stageData.LimitMin.x && transform.position.x < stageData.LimitMax.x &&
                    transform.position.y > stageData.LimitMin.y && transform.position.y < stageData.LimitMax.y)
                {
                    //�̵������� nextDirection���� ����
                    SetMoveDirection(nextDirection);
                    //nextDirection��none���� ����
                    nextDirection = Direction.None;
                    //coroutine����
                    break;
                }
            }

            yield return null;
        }
    }


    private Vector3 Vector3FromEnum(Direction state)
    {
        Vector3 direction = Vector3.zero;

        switch(state)
        {
            case Direction.Up:
                direction = Vector3.up;
                break;
            case Direction.Down:
                direction = Vector3.down;
                break;
            case Direction.Right:
                direction = Vector3.right;
                break;
            case Direction.Left:
                direction = Vector3.left;
                break;
        }
        return direction;
    }

}
