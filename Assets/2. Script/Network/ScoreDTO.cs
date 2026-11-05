using System;

// 1. Model Request untuk menyimpan skor (POST /api/score/save)
[Serializable]
public class SaveScoreRequest
{
    public int userId;
    public int score;
}

// 2. Model Response setelah menyimpan skor
[Serializable]
public class SaveScoreResponse
{
    public string status;
    public string message;
    public int scoreId;
}

// 3. Model Response saat mengambil skor tertinggi (GET /api/score/highscore/:userId)
[Serializable]
public class HighScoreResponse
{
    public string status;
    public int userId;
    public int highScore;
}