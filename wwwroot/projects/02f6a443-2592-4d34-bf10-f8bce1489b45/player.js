import { grimoire } from "./grimoire.js";

const maxMoveSpeed = 3.0;
const moveAcceleration = 0.2;

// const moveDirection = {
//     x: 0,
//     y: 0
// };

const moveSpeed = {
    x: 0.0,
    y: 0.0
};

export class Player {
    constructor(aGame){
        this.game = aGame;
        this.grimoire = grimoire;
        this.SetupInputDevice();
    }

    SetupInputDevice(){
        // This function needs to be called with ".call"; setting this to the current Game object
        this.input = {};
        this.input.keyUp = this.game.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.UP);
        this.input.keyDown = this.game.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.DOWN);
        this.input.keyLeft = this.game.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.LEFT);
        this.input.keyRight = this.game.input.keyboard.addKey(Phaser.Input.Keyboard.KeyCodes.RIGHT);
    }

    Preload(scene){
        scene.load.image("particle", `${__ProjectFolder}assets/particle.png`);
    }

    Create(scene){
        this.sprite = scene.add.sprite(400, 300, "particle");
    }

    Update(scene){

        if( (moveSpeed.x < 0 && moveSpeed.x + moveAcceleration > 0) || (moveSpeed.x < 0 && moveSpeed.x + moveAcceleration > 0) )
            moveSpeed.x = 0.0;
        else
            moveSpeed.x -= moveAcceleration;

        if( this.input.keyUp.isDown ){
            //this.sprite.y -= moveSpeed;
            if( moveSpeed.y > -maxMoveSpeed )
                moveSpeed.y -= moveAcceleration;
        }
        if( this.input.keyDown.isDown ){
            //this.sprite.y += moveSpeed;
            if( moveSpeed.y < maxMoveSpeed )
                moveSpeed.y += moveAcceleration;
        }
        if( this.input.keyLeft.isDown ){
            //this.sprite.x -= moveSpeed;
            if( moveSpeed.x > -maxMoveSpeed )
                moveSpeed.x -= moveAcceleration;
        }
        if( this.input.keyRight.isDown ){
            //this.sprite.x += moveSpeed;
            if( moveSpeed.x < maxMoveSpeed )
                moveSpeed.x += moveAcceleration;
        }

        this.sprite.x += moveSpeed.x;
        this.sprite.y += moveSpeed.y;
    }
};

export default {
    Player
};