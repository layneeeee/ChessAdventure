using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
    public bool en_passant = false;

    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);
        mIsFirstMove = true;
        mMovement = mColor == Color.white ? new Vector3Int(0,1,1) : new Vector3Int(0, -1, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
    }


    protected override void Move()
    {
        base.Move();
        if(mIsFirstMove){
            int offset = mColor == Color.white ? -1 : 1;
            int currentX = mCurrentCell.mBoardPosition.x;
            int currentY = mCurrentCell.mBoardPosition.y;
            CellState cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY + offset, this);
            if(cellState == CellState.Free && currentY != 2 && currentY != 5){
                en_passant = true;
                mPieceManager.CreateEnPassant(mColor, mCurrentCell.mBoard.mAllCells[currentX, (currentY + offset)]);
            }
        }
        mIsFirstMove = false;
        CheckForPromotion();
    }

    private bool MatchesState(int targetX, int targetY, CellState targetState)
    {    
        CellState cellState = CellState.None;
        cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);
        //Debug.Log(targetX + " " +  targetY + " - " + targetState + " = " + cellState);
        if(cellState == targetState){
            //Debug.Log("Aggiungo " + targetX + " " + targetY);
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
            return true;
        }
        return false;
    }

    private void CheckForPromotion()
    {
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        CellState cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY + mMovement.y, this);
        if(cellState == CellState.OutOfBounds){
            Color spriteColor = GetComponent<Image>().color;
            mPieceManager.PromotePiece(this, mCurrentCell, mColor, spriteColor);
        }
    }

    protected override void CheckPathing()
    {
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        MatchesState(currentX - mMovement.z, currentY + mMovement.z, CellState.Enemy);  

        MatchesState(currentX - mMovement.z, currentY + mMovement.z, CellState.EnPassant);  

        if(MatchesState(currentX, currentY + mMovement.y, CellState.Free)){
            if(mIsFirstMove){
                MatchesState(currentX, currentY + (mMovement.y * 2), CellState.Free);
            }
        }
        MatchesState(currentX + mMovement.z, currentY + mMovement.z, CellState.Enemy);

        MatchesState(currentX + mMovement.z, currentY + mMovement.z, CellState.EnPassant);  
    }

    public override string ToString(){
        return "Pawn " + mCurrentCell.mBoardPosition;
    }
}
