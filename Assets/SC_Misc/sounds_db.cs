using UnityEngine;
using System.Collections;

public class SoundManager
{

    //  обратный отсчет
    static public AudioSource count_5;    //U
    static public AudioSource count_4;    //U    
    static public AudioSource count_3;    //U
    static public AudioSource count_2;    //U
    static public AudioSource count_1;    //U
    static public AudioSource count_0;    //U

    //  наполнение энергии на старте
    static public AudioSource power_up;    //U

    //  взять батарейку
    static public AudioSource get_battery;    //U

    //  взять кристал прохождения
    static public AudioSource get_lup_jewel;    //U

    //  потерять кристалл прохождения
    static public AudioSource drop_lup_jewel;

    //  прохождение уровня
    static public AudioSource level_complete;

    //  счетчик поинтов после прохождения
    static public AudioSource point_counter;

    //  окончание подсчета поинтов
    static public AudioSource point_counter_end;

    //  звезда 1.2.3/   highScore
    static public AudioSource star_one;
    static public AudioSource star_two;
    static public AudioSource star_three;
    static public AudioSource high_score;

    //  продул
    static public AudioSource level_fail;

    //  если выпало 3 звезды.
    static public AudioSource all_stars;

    //  начало движения бота
    static public AudioSource bot_start_move;

    //  движение бота
    static public AudioSource bot_move;

    //  остановка бота
    static public AudioSource bot_stop;

    //  гоу!
    static public AudioSource go_fx;

    //  перемещение бота лево/право
    static public AudioSource bot_moving;    //U

    //  удар об ящик
    static public AudioSource bot_crash;    //U

    //  музыка меню
    static public AudioSource level_music;    //U

    //  музыка уровня
    static public AudioSource menu_music;    //U

    //  музыка boost
    static public AudioSource boost_music;    //U

    //  клик по кнопке
    static public AudioSource click_common;    //U


    static public AudioSource get_coin;    //U
    static public AudioSource get_coin_sum;    //U
    static public AudioSource get_bullet;    //U
    static public AudioSource impact;    //U
    static public AudioSource shoot;    //U
    static public AudioSource alarm_boss_prepare;    //U
    static public AudioSource winner;    //U
    static public AudioSource get_full_stack;
    

    //
    //  загрузка звуковых префабов 
    //
    

    static public void Init()
    {
        /*
        Debug.Log("NOTE> sound off");
        AudioListener.volume = 0;
        */


        get_full_stack = Load ("zapsplat_bonus"); 
        get_coin = Load("get_coin");
        get_bullet = Load("shoot_m"); //---------------------------- заменить
        impact = Load("impact");
        shoot = Load("shoot_m");
        alarm_boss_prepare = Load("alarm");
        winner = Load("hit_victory");
        get_coin_sum = Load ("coin_b");



        count_0 = Load("countdown/count0");
        count_1 = Load("countdown/count1");
        count_2 = Load("countdown/count2");
        count_3 = Load("countdown/count3");
        count_4 = Load("countdown/count4");
        count_5 = Load("countdown/count5");
        power_up = Load("powers/power_up");
        get_battery = Load("powers/get_battery");
        get_lup_jewel = Load("powers/get_lup_jewel");
        drop_lup_jewel = Load("powers/drop_lup_jewel");
        level_complete = Load("music/level_complete");
        point_counter = Load("point_counter");
        point_counter_end = Load("point_counter_end");
        star_one = Load("stars/star_one");
        star_two = Load("stars/star_two");
        star_three = Load("stars/star_three");
        high_score = Load("stars/highscore");
        level_fail = Load("stars/level_fail");
        all_stars = Load("stars/all_stars");
        bot_start_move = Load("bot/bot_start_move");

        //  FIXIT: prefab name
        bot_move = Load("bot/engine");

        bot_stop = Load("bot/bot_stop");
        go_fx = Load("go_fx");
        bot_moving = Load("bot/bot_moving");
        bot_crash = Load("bot/bot_crash");
        level_music = Load("music/level_music");
        boost_music = Load("music/boost_music");
        menu_music = Load("music/menu_music");
        click_common = Load("click_common");
        /*
        inst = new GameObject();
        inst.AddComponent<snd_entity>();
        inst.SetActive(false);
        */
    }
    /*
    static public void CreateSndInst(AudioSource _as, float _delay, float _ttl)
    {
        GameObject go = (GameObject)GameObject.Instantiate(inst);
        var se = go.GetComponent<snd_entity>();
        if (se == null)
            se = go.AddComponent<snd_entity>();
        se.a = _as;
        se.delay = _delay;
        se.ttl = _ttl;
        go.SetActive(true);
    }
    */
    static AudioSource Load(string name)
    {
        try
        {
            return (AudioSource)GameObject.Instantiate(Resources.Load(name, typeof(AudioSource)));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Не могу найти звук: " + name + ". exp: " + e.ToString());
            Debug.LogWarning("Проверь путь. Создан ли префаб.");
            return null;
        }
    }
}
