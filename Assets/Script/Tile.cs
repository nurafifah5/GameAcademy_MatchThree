using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 finalPosition;
    private float swipeAngle;
    private Vector3 tempPosition;
    //data posisi tile
    public float xPosition;
    public float yPosition;
    public int column;
    public int row;
    private Grid grid;
    private GameObject otherTile;
    public bool isMatched = false;
    private int previousColumn;
    private int previousRow;

    void Start()
    {
        //menentukan posisi dari tile
        grid = FindObjectOfType<Grid>();
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        row = Mathf.RoundToInt((xPosition - grid.startPos.x) / grid.offset.x);
        column = Mathf.RoundToInt((yPosition - grid.startPos.y) / grid.offset.y);
    }

    void Update()
    {
        //xPosition = (column * grid.offset.x) + grid.startPos.x;
        //yPosition = (row * grid.offset.y) + grid.startPos.y;
        //SwipeTile();

        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;
        }
    }

    void OnMouseDown()
    {
        //mendapatkan titik awal sentuhan jari
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        //mendapatkan titik akhir sentuhan jarii
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalcuteAngle();
    }

    void CalcuteAngle()
    {
        //menghitung sudut antar posisi awal dan posisi akhir
        swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * 180 / Mathf.PI;
        MoveTile();
    }

    void MoveTile()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < grid.gridSizeX)
        {
            //right swipe
            Debug.Log("Right Swipe");
            SwipeRightMove();
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < grid.gridSizeY) {
            //up swipe
            Debug.Log("Up Swipe");
            SwipeUpMove();
        } else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            //left swipe
            Debug.Log("left swipe");
            SwipeLeftMove();
        } else if(swipeAngle < -45 && swipeAngle >= -135 && row >0)
        {
            //down swipe
            Debug.Log("down swipe");
            SwipeDownMove();
        }
        //memeriksa kecocokan tile setelah ada tile yang di swap. jika tidak match akan dikembalikan ke posisi awal
        StartCoroutine(checkMove());
    }

    void SwipeTile()
    {
        if (Mathf.Abs(xPosition - transform.position.x) > .1)
        {
            //berpindah ke arah target
            Vector3 tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //menetapkan posisi
            Vector3 tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }

        if (Mathf.Abs(yPosition - transform.position.y) > .1)
        {
            //berpindah ke arah target
            Vector3 tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            //menetapkan posisi
            Vector3 tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }
    }

    void SwipeRightMove()
    {
        if(column + 1 < grid.gridSizeX)
        {
            //menukar posisi tile dengan sebelah kanannya
            otherTile = grid.tiles[column + 1, row];
            otherTile.GetComponent<Tile>().column -= 1;
            column += 1;
        }
    }
    
    void SwipeUpMove()
    {
        if(row + 1 < grid.gridSizeY)
        {
            //menukar posisi tile dengan sebelah atasnya
            otherTile = grid.tiles[column, row + 1];
            otherTile.GetComponent<Tile>().row -= 1;
            row += 1;
        }
    }

    void SwipeLeftMove()
    {
        if(column - 1 >= 0)
        {
            //menukar posisi tile dengan sebelah kirinya
            otherTile = grid.tiles[column - 1, row];
            otherTile.GetComponent<Tile>().column += 1;
            column -= 1;
        }
    }

    void SwipeDownMove()
    {
        if(row - 1 >= 0)
        {
            //menukar posisi tile dengan sebelah bawahnya
            otherTile = grid.tiles[column, row - 1];
            otherTile.GetComponent<Tile>().row += 1;
            row -= 1;
        }
    }

    void CheckMatches()
    {
        //memeriksa kesamaan horizontal
        if(column > 0 && column< grid.gridSizeX - 1)
        {
            //memeriksa samping kiri dan kanan
            GameObject leftTile = grid.tiles[column - 1, row];
            GameObject rightTile = grid.tiles[column + 1, row];
            if(leftTile != null && rightTile != null)
            {
                if(leftTile.CompareTag(gameObject.tag) && rightTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    rightTile.GetComponent<Tile>().isMatched = true;
                    leftTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }

        //memeriksa kesamaan vertikal
        if(row > 0 && row < grid.gridSizeY - 1)
        {
            //memeriksa samping atas dan bawah
            GameObject upTile = grid.tiles[column, row + 1];
            GameObject downTile = grid.tiles[column, row - 1];
            if(upTile != null && downTile != null)
            {
                if(upTile.CompareTag(gameObject.tag) && downTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    downTile.GetComponent<Tile>().isMatched = true;
                    upTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }
    }

    IEnumerator checkMove()
    {
        yield return new WaitForSeconds(.5f);
        //cek jika tilenya tidak sama kembalikan, jika ada yang sama maka panggil destroyMatches
        if(otherTile != null)
        {
            if(!isMatched && !otherTile.GetComponent<Tile>().isMatched)
            {
                otherTile.GetComponent<Tile>().row = row;
                otherTile.GetComponent<Tile>().column = column;
                row = previousRow;
                column = previousColumn;
            } else
            {
                grid.DestroyMatches();
            }
        }
        otherTile = null;
    }
}
