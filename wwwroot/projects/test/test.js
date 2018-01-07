var TestGame = {};

TestGame.Main = function(game){};
TestGame.Main.Prototype = {
    init: function(){
        // do some init stuff
    },

    preload: function(){
        // do some preload stuff
    },

    create: function(){
        // what is the difference between preload and create?
    },

    update: function(){
        // updates 30x per second
    }
};

window.onload = function(){
    var game = new Phaser.Game(800, 600, Phaser.AUTO, "gameContainer");
    game.state.add("main", TestGame.Main);
    game.state.start("main");
}
