const string BACKGROUND = "Images/Background/BG_04.png";
const string SURFACE = "Images/surface.png";
const string SURFACE_DANGER = "Images/surface-danger.png";
const string CANDY = "Images/candy.png";
const string HERO_CHARACTER = "Girl";

int backgroundWidth = 0, backgroundHeight = 0;
int surfaceHeight = 0, surfaceWidth = 0, surfaceDangerHeight = 0;
int candyWidth = 0, candyHeight = 0;

bool isDead = false;
bool won = false;

float surfaceMovement = 0;
const float SURFACE_MOVEMENT_SPEED = 5f;

int heroHeightWalk = 0, heroHeightDie = 0;
int heroWidthWalk = 0, heroWidthDie = 0;
const int HERO_WALK_FRAMES = 15, HERO_DIE_FRAMES = 15;
const float HERO_HITBOX_REDUCTION = 45f;
float heroX = 0f;
float heroY = 0f;
float heroYVelocity = 0f;
const float JUMP_VELOCITY = 20f;
const float GRAVITY = 2f;

float animationFrame = 0;
float ANIMATION_SPEED = 0.5f;

void Setup(P5 p5)
{
    p5.Background("#00ccff");

    P5Image background = p5.LoadImage(BACKGROUND);
    backgroundHeight = background.Height;
    backgroundWidth = background.Width;

    P5Image surface = p5.LoadImage(SURFACE);
    surfaceHeight = surface.Height;
    surfaceWidth = surface.Width;

    P5Image candy = p5.LoadImage(CANDY);
    candyHeight = candy.Height;
    candyWidth = candy.Width;

    P5Image surfaceDanger = p5.LoadImage(SURFACE_DANGER);
    surfaceDangerHeight = surfaceDanger.Height;

    var heroWalk = LoadImages(p5, "Walk", HERO_WALK_FRAMES);
    heroHeightWalk = heroWalk.Height;
    heroWidthWalk = heroWalk.Width;

    var heroDie = LoadImages(p5, "Die", HERO_DIE_FRAMES);
    heroHeightDie = heroDie.Height;
    heroWidthDie = heroDie.Width;
}

void Draw(P5 p5)
{
    DrawBackground(p5);

    heroY += heroYVelocity;
    heroYVelocity -= GRAVITY;
    if (heroY < 0) { heroY = 0; heroYVelocity = 0; }

    float heroLeft = heroX;
    float heroTop = 0f;
    if (!isDead && !won)
    {
        if (p5.KeyIsDown(Key.Left)) { heroX = Math.Max(0f, heroX - 10f); }
        else if (p5.KeyIsDown(Key.Right)) { heroX = Math.Min(p5.Width - heroWidthWalk, heroX + 10f); }

        heroTop = p5.Height - surfaceHeight - heroHeightWalk - heroY;
        p5.Image(GetHeroImageName(animationFrame, "Walk"), heroLeft, heroTop);
        animationFrame = (animationFrame + ANIMATION_SPEED) % HERO_WALK_FRAMES;

        p5.Stroke("Red");
        p5.Rect(heroLeft + HERO_HITBOX_REDUCTION, heroTop, heroWidthWalk - HERO_HITBOX_REDUCTION * 2, heroHeightWalk);
    }
    else if (!won && animationFrame < HERO_DIE_FRAMES)
    {
        heroTop = p5.Height - surfaceHeight - heroHeightDie - heroY;
        p5.Image(GetHeroImageName(animationFrame, "Die"), heroLeft, heroTop + 60);
        if (animationFrame < HERO_DIE_FRAMES)
        {
            animationFrame = animationFrame + ANIMATION_SPEED;
        }
    }

    for (int i = 0; i < 150; i++)
    {
        float surfaceLeft = i * surfaceWidth - surfaceMovement;
        if (i == 100)
        {
            float candyTop = p5.Height - surfaceHeight - candyHeight;
            p5.Image(CANDY, surfaceLeft, candyTop);

            if (p5.DoesCollide(heroLeft + HERO_HITBOX_REDUCTION, heroTop, heroWidthWalk - HERO_HITBOX_REDUCTION * 2, heroHeightWalk,
                surfaceLeft, candyTop, candyWidth, candyHeight) && !isDead && !won)
            {
                won = true;
            }
        }

        if (i % 15 == 0 && i > 0 && !isDead && !won)
        {
            float surfaceTop = p5.Height - surfaceDangerHeight;
            p5.Image(SURFACE_DANGER, surfaceLeft, surfaceTop);

            if (p5.DoesCollide(heroLeft + HERO_HITBOX_REDUCTION, heroTop, heroWidthWalk - HERO_HITBOX_REDUCTION * 2, heroHeightWalk,
                surfaceLeft, surfaceTop, surfaceWidth, surfaceDangerHeight) && !isDead && !won)
            {
                isDead = true;
                animationFrame = 0;
            }
        }
        else
        {
            p5.Image(SURFACE, surfaceLeft, p5.Height - surfaceHeight);
        }
    }

    if (!isDead)
    {
        if (won)
        {
            p5.TextSize(100);
            p5.Text("You won!", p5.Width / 2, p5.Height / 2);
        }

        surfaceMovement += SURFACE_MOVEMENT_SPEED;
    }
    else if (isDead)
    {
        p5.TextSize(100);
        p5.Text("Game Over!", p5.Width / 2, p5.Height / 2);
    }
}

void KeyDown(P5 p5, Key key)
{
    if (isDead || won) { return; }

    switch (key)
    {
        case Key.Space: heroYVelocity = JUMP_VELOCITY; break;
        default: break;
    }
}

void DrawBackground(P5 p5)
{
    float ratio = backgroundHeight / p5.Height;
    p5.Image(BACKGROUND, 0, 0, backgroundWidth / ratio, p5.Height);
}

string GetHeroImageName(float frame, string mode)
{
    return $"Images/{HERO_CHARACTER}/{mode}_0{(int)frame + 1:00}.png";
}

P5Image LoadImages(P5 p5, string mode, int frames)
{
    P5Image image = p5.LoadImage(GetHeroImageName(0, mode));
    for (int i = 1; i < frames; i++) { p5.LoadImage(GetHeroImageName(i, mode)); }
    return image;
}

GameApplication.Run(new(
    Setup: Setup,
    Draw: Draw,
    KeyDown: KeyDown
));
