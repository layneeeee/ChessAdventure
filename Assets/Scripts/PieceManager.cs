using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool mIsKingAlive = true;
    public bool whiteTurn = true;
    public GameObject mPiecePrefab;

    private List<BasePiece> mWhitePieces = null;
    private List<BasePiece> mBlackPieces = null;
    private List<BasePiece> mPromotedPieces = new List<BasePiece>();
    public List<BasePiece> enPassantPieces = new List<BasePiece>();
    private bool addedEnPassant = false;

    bool isCheck(){
        if(whiteTurn){
            BasePiece whiteKing = mWhitePieces.Find(x => x.pieceName == "King");
            Debug.Log(whiteKing);
        }else{
            
        }
        return true;
    }

    public void UpdateEnPassant(){
        if(addedEnPassant){
            enPassantPieces[0].Kill();
            Destroy(enPassantPieces[0].gameObject);
            enPassantPieces.Remove(enPassantPieces[0]);
            addedEnPassant = false;
        }
    }

    private string[] mPieceOrder = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R"
    };

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"P",  typeof(Pawn)},
        {"R",  typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B",  typeof(Bishop)},
        {"K",  typeof(King)},
        {"Q",  typeof(Queen)}
    };

    public void Setup(Board board)
    {
        mWhitePieces = CreatePieces(Color.white, new Color32(80, 124, 159, 255), board);
        mBlackPieces = CreatePieces(Color.black, new Color32(210, 95, 64, 255), board);

        PlacePieces(1, 0, mWhitePieces, board);
        PlacePieces(6, 7, mBlackPieces, board);

        SwitchSides(Color.black);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for(int i=0; i<mPieceOrder.Length; i++){
            string key = mPieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            BasePiece newPiece = CreatePiece(pieceType);
            newPieces.Add(newPiece);

            newPiece.Setup(teamColor, spriteColor, this);
        }
        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        GameObject newPieceObject = Instantiate(mPiecePrefab);
        newPieceObject.transform.SetParent(transform);

        newPieceObject.transform.localScale = new Vector3(1,1,1);
        newPieceObject.transform.localRotation = Quaternion.identity;
        
        BasePiece newPiece = (BasePiece) newPieceObject.AddComponent(pieceType);
        newPiece.pieceName = pieceType.ToString();
        return newPiece;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
    {
        for(int i=0; i<8; i++){
            pieces[i].Place(board.mAllCells[i, pawnRow]);
            pieces[i+8].Place(board.mAllCells[i, royaltyRow]);
        }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value)
    {
        foreach(BasePiece piece in allPieces){
            piece.enabled = value;
        }
    }

    public void SwitchSides(Color color)
    {
        if(isCheck()){
            Debug.Log("SCACCO");
        }
        if(!mIsKingAlive){
            ResetPieces();
            mIsKingAlive = true;
            color = Color.black;
        }
        bool isBlackTurn = color == Color.white;
        whiteTurn = !isBlackTurn;
        SetInteractive(mWhitePieces, !isBlackTurn);
        SetInteractive(mBlackPieces, isBlackTurn);

        foreach(BasePiece piece in mPromotedPieces){
            bool isBlackPiece = piece.mColor != Color.white ? true : false;
            bool isPartOfTeam = isBlackPiece ? isBlackTurn : !isBlackTurn;
            piece.enabled = isPartOfTeam;
        }
    }

    public void ResetPieces()
    {
        foreach(BasePiece piece in mPromotedPieces){
            piece.Kill();
            Destroy(piece.gameObject);
        }

        foreach(BasePiece piece in enPassantPieces){
            piece.Kill();
            Destroy(piece.gameObject);
        }

        foreach(BasePiece piece in mWhitePieces){
            piece.Reset();
        }

        foreach(BasePiece piece in mBlackPieces){
            piece.Reset();
        }
    }

    public void PromotePiece(Pawn pawn, Cell cell, Color teamColor, Color spriteColor)
    {
        pawn.Kill();

        BasePiece promotedPiece = CreatePiece(typeof(Queen));
        promotedPiece.Setup(teamColor, spriteColor, this);

        promotedPiece.Place(cell);
        mPromotedPieces.Add(promotedPiece);
    }

    public void CreateEnPassant(Color color, Cell cell){
        BasePiece newPiece = CreatePiece(typeof(Pawn));
        Color32 color32 = new Color32(80, 124, 159, 255);
        color32.a = 0;
        newPiece.Setup(color, color32, this);
        newPiece.Place(cell);
        newPiece.isEnPassant = true;
        
        enPassantPieces.Add(newPiece);
        addedEnPassant = true;
    }
}
