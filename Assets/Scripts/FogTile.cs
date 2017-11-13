using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogTile {

    public bool unknown;
    public GameObject model;
    public BlockExploration blockExploration;
    
    public void Destroy() {
        
        if(!this.unknown) {
            return;
        }

        this.unknown = false;
        this.blockExploration.DecreaseCount();

        if (model != null) {
            GameController.DestroyImmediate(this.model);
            this.model = null;
        }
    }

}
