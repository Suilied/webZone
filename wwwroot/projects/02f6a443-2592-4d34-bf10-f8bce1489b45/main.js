{/* <script type="module" src="~/projects/02f6a443-2592-4d34-bf10-f8bce1489b45/player.js" asp-append-version="true">
</script>

<script type="module" src="~/projects/02f6a443-2592-4d34-bf10-f8bce1489b45/main.js" asp-append-version="true">
</script> */}

import { Player } from "./player.js";

const config = {
    type: Phaser.AUTO,
    width: 800,
    height: 600,
    physics: {
        default: "arcade",
        arcade: {
            gravity: { y: 200 }
        }
    },
    scene: {
        preload: preload,
        create: create,
        update: update
    }
};

const game = new Phaser.Game(config);
const player = new Player(game);
console.log("Player obj --", player);

function preload ()
{
    this.load.image("sky", `${__ProjectFolder}assets/sky.png`);
    player.Preload(this);
}

function create ()
{
    this.add.image(400, 300, "sky");
    player.Create(this);

    // const particles = this.add.particles("particle");
    // const emitter = particles.createEmitter({
    //     x: 400,
    //     y: 300,
    //     speed: 100,
    //     scale: { start: 1, end: 0 },
    //     // blendMode: "ADD"
    // });
}

function update ()
{
    player.Update(this);
}