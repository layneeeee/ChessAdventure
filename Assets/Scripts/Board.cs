﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// New
public enum CellState
{
    None,
    Friendly,
    Enemy,
    Free,
    OutOfBounds, 
    EnPassant
}

public class Board : MonoBehaviour
{
    public GameObject mCellPrefab;

    [HideInInspector]
    public Cell[,] mAllCells = new Cell[8, 8];

    public void Create()
    {
        for(int y=0; y<8; y++){
            for(int x=0; x<8; x++){
                GameObject newCell = Instantiate(mCellPrefab, transform);
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x*100) + 50, (y*100) + 50);
                mAllCells[x,y] = newCell.GetComponent<Cell>();
                mAllCells[x,y].Setup(new Vector2Int(x,y), this);
            }
        }

        for(int x=0; x<8; x+=2){
            for(int y=0; y<8; y++){
                int offset = (y%2 != 0) ? 0 : 1;
                int finalX = x+offset;

                mAllCells[finalX, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            }
        }
    }

    public CellState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {   
        //PrintBoard();
        if(targetX < 0 || targetX > 7){
            return CellState.OutOfBounds;
        }
        if(targetY < 0 || targetY > 7){
            return CellState.OutOfBounds;
        }
        Cell targetCell = mAllCells[targetX, targetY];
        if(targetCell.mCurrentPiece != null){
            //Debug.Log(targetX + " - " + targetY + " --- " + targetCell.mCurrentPiece.mColor);
            if(checkingPiece.mColor == targetCell.mCurrentPiece.mColor){
                return CellState.Friendly;
            }
            if(checkingPiece.mColor != targetCell.mCurrentPiece.mColor && targetCell.mCurrentPiece.isEnPassant){
                return CellState.EnPassant;
            }
            if(checkingPiece.mColor != targetCell.mCurrentPiece.mColor){
                return CellState.Enemy;
            }
        }
        return CellState.Free;
    }

    public void PrintBoard(){
        for(int i=0; i<8; i++){
            for(int j=0; j<8; j++){
                Debug.Log(mAllCells[i,j].mCurrentPiece);
            }
        }
    }
}
