using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // 游戏场景枚举，便于安全启动场景
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        GameOverScene,
    }

    //加载场景 
    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
