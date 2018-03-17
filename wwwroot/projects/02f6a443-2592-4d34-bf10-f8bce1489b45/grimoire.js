export const grimoire = {
    name: "simple-grimoire",
    pages: []
};

/* 4 cardinal directions */
grimoire.pages.push( {
    name: "hate",
    le: "annoyance",
    up: "aggresiveness",
    ri: "",
    do: "contempt"
} );

grimoire.pages.push( {
    name: "extasy",
    le: "optimism",
    up: "serenity",
    ri: "love",
    do: ""
} );

grimoire.pages.push( {
    name: "terror",
    le: "",
    up: "submission",
    ri: "apprehension",
    do: "awe"
} );

grimoire.pages.push( {
    name: "grief",
    le: "remorse",
    up: "",
    ri: "disapproval",
    do: "pensiveness"
} );


/* LEFT - ANGER */
grimoire.pages.push( {
    name: "annoyance",
    le: "",
    up: "aggresiveness",
    ri: "hate",
    do: "contempt"
} );

grimoire.pages.push( {
    name: "aggresiveness",
    le: "",
    up: "optimism",
    ri: "extasy",
    do: "hate"
} );

grimoire.pages.push( {
    name: "contempt",
    le: "annoyance",
    up: "aggresiveness",
    ri: "hate",
    do: ""
} );


/* UP - JOY */
grimoire.pages.push( {
    name: "optimism",
    le: "",
    up: "serenity",
    ri: "love",
    do: "extasy"
} );

grimoire.pages.push( {
    name: "serenity",
    le: "optimism",
    up: "",
    ri: "love",
    do: "extasy"
} );

grimoire.pages.push( {
    name: "love",
    le: "optimism",
    up: "serenity",
    ri: "",
    do: "extasy"
} );


/* DOWN - SADNESS */
grimoire.pages.push( {
    name: "remorse",
    le: "",
    up: "grief",
    ri: "disapproval",
    do: "pensiveness"
} );

grimoire.pages.push( {
    name: "pensiveness",
    le: "remorse",
    up: "grief",
    ri: "disapproval",
    do: ""
} );

grimoire.pages.push( {
    name: "disapproval",
    le: "remorse",
    up: "grief",
    ri: "",
    do: "pensiveness"
} );

/* RIGHT - FEAR */
grimoire.pages.push( {
    name: "submission",
    le: "terror",
    up: "",
    ri: "apprehension",
    do: "awe"
} );

grimoire.pages.push( {
    name: "awe",
    le: "terror",
    up: "submission",
    ri: "apprehension",
    do: ""
} );

grimoire.pages.push( {
    name: "apprehension",
    le: "terror",
    up: "submission",
    ri: "",
    do: "awe"
} );

export default {
    grimoire
};