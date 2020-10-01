using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEvent : MonoBehaviour
{
        //abstract class untuk base event dari tile
        //apa yang terjadi jika tile match
        public abstract void OnMatch();
        //memeriksa jika persyaratan event telah terpenuhi
        public abstract bool AchievementCompleted();
}
