using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Загрузка сцен 
/// </summary>
public static class SceneLoader
{
    // Загрузить "Перезагрузка игры"
    public static void Intro()
    {
        SceneManager.LoadScene("a_loader");
    }

    // Загрузить "ПОБЕДА"
    public static void Winner()
    {
        SceneManager.LoadScene("a_winner");
    }

    // Загрузить "СТАДИЯ"
    public static void Game()
    {
        SceneManager.LoadScene("a_game");
    }

    // Загрузить "РЕДАКТОР"
    public static void Editor()
    {
        SceneManager.LoadScene("a_map_editor");
    }

    // Выйти в "ГЛАВНОЕ МЕНЮ"
    public static void Menu()
    {
        SceneManager.LoadScene("a_start");
    }
}
