using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //ukuran grid
    public int gridSizeX, gridSizeY;
    public Vector2 startPos, offset;
    //prefab background grid
    public GameObject tilePrefab;
    //array 2 dimensi untuk membuat tile
    public GameObject[,] tiles;
    public GameObject[] candies;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new GameObject[gridSizeX, gridSizeY];
        CreateGrid();
    }

    void CreateGrid()
    {
        //menentukan offset, didapatkan dari size prefab
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        //menentukan posisi awal
        startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2)) + (Vector3.down * (offset.y * gridSizeY / 3));
        //looping membuat tile
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                GameObject backgroundTile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + "," + y + ")";

                int index = Random.Range(0, candies.Length);

                //lakukan iterasi sampai tile tidak ada yang sama dengan sebelahnya
                int MAX_ITERATION = 0;
                while (MatchesAt(x, y, candies[index]) && MAX_ITERATION < 100)
                {
                    index = Random.Range(0, candies.Length);
                    MAX_ITERATION++;
                }
                MAX_ITERATION = 0;

                //create object tanpa pool
                //GameObject candy = Instantiate(candies[index], pos, Quaternion.identity);
                //create object dengan pool
                GameObject candy = ObjectPooler.Instance.SpawnFromPool(index.ToString(), pos, Quaternion.identity);

                //candy.GetComponent<Tile>().Init();
                candy.name = "(" + x + "," + y + ")";
                tiles[x, y] = candy;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        //cek jika ada tile yang sama dengan dibawah dan disampingnya
        if(column > 1 && row > 1)
        {
            if(tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if(tiles[column, row - 1].tag == piece.tag && tiles[column, row-2].tag == piece.tag)
            {
                return true;
            }
        }
        //cek jika ada tile yang sama dengan diatas dan sampingnya
        else if (column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            } 
            if(column > 1)
            {
                if(tiles[column -1, row].tag == piece.tag && tiles[column -2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        //menghancurkan tile pada indeks tertentu
        if(tiles[column, row].GetComponent<Tile>().isMatched)
        {
            GameManager.instance.GetScore(10);

            //menghancurkan tile tanpa menggunakan pool
            //Destroy(tiles[column, row]);
            //menghancurkan tile dengan menggunakan pool
            GameObject gm = tiles[column, row];
            gm.SetActive(false);

            tiles[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        //melakukan looping untuk memeriksa tile yang kondisinya matched lalu di destroy
        for(int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                DestroyMatchesAt(i, j);
            }
        }
        StartCoroutine(DecreaseRow());
    }

    //mendeteksi tile indeks mana yang kosong(null). Jika ada maka akan dibuat candy baru di lokasi tsb secara random
    private void RefillBoard()
    {
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                if(tiles[x,y] == null)
                {
                    Vector2 tempPosition = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                    int candyToUse = Random.Range(0, candies.Length);

                    //create object tanpa pool
                    //GameObject tileToRefill = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                    //create object dengan pool
                    GameObject tileToRefill = ObjectPooler.Instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity);

                    //tileToRefill.GetComponent<Tile>().Init();
                    tiles[x, y] = tileToRefill;
                }
            }
        }
    }

    //mengecek setiap grid
    private bool MatchesOnBoard()
    {
        for(int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                if(tiles[i,j] != null)
                {
                    if (tiles[i, j].GetComponent<Tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //melakukan pengurangan row dari tile yang hilang
    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for(int i = 0; i < gridSizeX; i++)
        {
            for(int j = 0; j < gridSizeY; j++)
            {
                if(tiles[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    tiles[i, j].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    //mengisi tile dalam board
    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
}
