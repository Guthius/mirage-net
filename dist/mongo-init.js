db.accounts.drop();
db.accounts.insertMany([
    {
        _id: '682338f5487ac494f50c0915',
        name: 'admin',
        password: '$2a$11$6mVF.MEb51gVzeBFLeCGHOUiIoVKW808BunbpEOmT6BoVtlqnjfLa'
    }
]);

db.classes.drop();
db.classes.insertMany([
    {
        _id: '68234080be15fb3f0f2d4cac',
        defense: 7,
        intelligence: 0,
        name: 'Knight',
        speed: 5,
        sprite: 0,
        strength: 8
    },
    {
        _id: '68234080be15fb3f0f2d4cad',
        defense: 2,
        intelligence: 13,
        name: 'Black Mage',
        speed: 3,
        sprite: 1,
        strength: 2
    },
    {
        _id: '68234080be15fb3f0f2d4cae',
        defense: 5,
        intelligence: 8,
        name: 'Monk',
        speed: 7,
        sprite: 30,
        strength: 8
    }
]);

db.items.drop();
db.items.insertMany([
    {
        _id: 1,
        data1: 0,
        data2: 0,
        data3: 0,
        name: "Gold",
        sprite: 0,
        type: 12
    },
    {
        _id: 2,
        data1: 180,
        data2: 2,
        data3: 0,
        name: "Tarnished Dagger",
        sprite: 13,
        type: 1
    },
    {
        _id: 3,
        data1: 30,
        data2: 0,
        data3: 0,
        name: "Healing Potion",
        sprite: 25,
        type: 5
    },
    {
        _id: 4,
        data1: 80,
        data2: 2,
        data3: 0,
        name: "Rusty Mail",
        sprite: 5,
        type: 2
    },
    {
        _id: 5,
        data1: 100,
        data2: 3,
        data3: 0,
        name: "Steel Dagger",
        sprite: 10,
        type: 1
    }
]);


db.npcs.drop();
db.npcs.insertMany([
    {
        _id: 1,
        attack_say: "arrrrrrr...... ughhhh *puke*",
        behavior: 1,
        defense: 1,
        drop_chance: 1,
        drop_item_id: 1,
        drop_item_quantity: 3,
        intelligence: 0,
        name: "Drunken Sea Pirate",
        range: 1,
        spawn_secs: 5,
        speed: 5,
        sprite: 7,
        strength: 2
    },
    {
        _id: 2,
        attack_say: "'Ack, leave me alone!!!!",
        behavior: 1,
        defense: 3,
        drop_chance: 2,
        drop_item_id: 1,
        drop_item_quantity: 5,
        intelligence: 1,
        name: "Cloaked Imp",
        range: 10,
        spawn_secs: 2,
        speed: 4,
        sprite: 8,
        strength: 3
    },
    {
        _id: 3,
        attack_say: "Your gold or your life chum!",
        behavior: 0,
        defense: 4,
        drop_chance: 4,
        drop_item_id: 1,
        drop_item_quantity: 10,
        intelligence: 0,
        name: "Bandit",
        range: 3,
        spawn_secs: 5,
        speed: 2,
        sprite: 6,
        strength: 3
    }
]);

db.shops.drop();
db.shops.insertMany([
    {
        _id: 1,
        fixes_items: true,
        join_say: "Hello adventurer, I have some cheap items for sale.  I can also fix your damaged equipment.",
        leave_say: "Good luck on your journey.",
        name: "Zjin",
        trades: [
            {
                give_item_id: 0,
                give_item_quantity: 0,
                get_item_id: 0,
                get_item_quantity: 0
            },
            {
                give_item_id: 1,
                give_item_quantity: 10,
                get_item_id: 2,
                get_item_quantity: 1
            },
            {
                give_item_id: 1,
                give_item_quantity: 100,
                get_item_id: 5,
                get_item_quantity: 1
            },
            {
                give_item_id: 1,
                give_item_quantity: 50,
                get_item_id: 4,
                get_item_quantity: 1
            },
            {
                give_item_id: 2,
                give_item_quantity: 1,
                get_item_id: 1,
                get_item_quantity: 5
            },
            {
                give_item_id: 5,
                give_item_quantity: 1,
                get_item_id: 1,
                get_item_quantity: 50
            },
            {
                give_item_id: 4,
                give_item_quantity: 1,
                get_item_id: 1,
                get_item_quantity: 25
            },
            {
                give_item_id: 0,
                give_item_quantity: 0,
                get_item_id: 0,
                get_item_quantity: 0
            },
            {
                give_item_id: 0,
                give_item_quantity: 0,
                get_item_id: 0,
                get_item_quantity: 0
            }
        ]
    }
]);

db.spells.drop();
db.spells.insertMany([
    {
        _id: 1,
        data1: 10,
        data2: 0,
        data3: 0,
        name: "Minor Heal",
        req_class_id: '',
        req_level: 0,
        type: 0
    }
]);
