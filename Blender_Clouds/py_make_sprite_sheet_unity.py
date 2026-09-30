from PIL import Image, ImageOps
from pathlib import Path


# ============================================================
# SETTINGS
# ============================================================

INPUT_FOLDER = Path("blender_render")
#INPUT_FOLDER = Path("images")
OUTPUT_FILE = Path("sprite_sheet.png")

# Sprite frame resolution
FRAME_WIDTH = 64
FRAME_HEIGHT = 64

# Grid
COLUMNS = 8
ROWS = 8
IMAGE_COUNT = 64

# Space between frames
PADDING = 0

# Space around the outside of the entire sprite sheet
OUTER_MARGIN = 0

# Background color
# (0, 0, 0, 0) = transparent
BACKGROUND_COLOR = (0, 0, 0, 0)


# ============================================================
# IMAGE PROCESSING
# ============================================================

def resize_and_crop(image, size):
    """
    Resize image while preserving aspect ratio,
    then center-crop it to exactly the requested size.
    """

    image = image.convert("RGBA")

    # ImageOps.fit scales the image to completely fill
    # the target size and crops the excess.
    return ImageOps.fit(
        image,
        size,
        method=Image.Resampling.LANCZOS,
        centering=(0.5, 0.5)
    )


# ============================================================
# SPRITE SHEET CREATION
# ============================================================

def create_sprite_sheet():

    # --------------------------------------------------------
    # Find images
    # --------------------------------------------------------

    images = sorted(
        [
            p for p in INPUT_FOLDER.iterdir()
            if p.suffix.lower() in [
                ".png",
                ".jpg",
                ".jpeg",
                ".bmp",
                ".webp"
            ]
        ]
    )

    if len(images) != IMAGE_COUNT:
        raise ValueError(
            f"Expected {IMAGE_COUNT} images, "
            f"but found {len(images)}"
        )

    # --------------------------------------------------------
    # Calculate sprite sheet dimensions
    # --------------------------------------------------------

    sheet_width = (
        OUTER_MARGIN * 2
        + COLUMNS * FRAME_WIDTH
        + (COLUMNS - 1) * PADDING
    )

    sheet_height = (
        OUTER_MARGIN * 2
        + ROWS * FRAME_HEIGHT
        + (ROWS - 1) * PADDING
    )

    # --------------------------------------------------------
    # Create empty sprite sheet
    # --------------------------------------------------------

    sheet = Image.new(
        "RGBA",
        (sheet_width, sheet_height),
        BACKGROUND_COLOR
    )

    # --------------------------------------------------------
    # Add images
    # --------------------------------------------------------

    for index, image_path in enumerate(images):

        print(
            f"Processing {index + 1}/{IMAGE_COUNT}: "
            f"{image_path.name}"
        )

        image = Image.open(image_path)

        # Resize + center crop
        image = resize_and_crop(
            image,
            (FRAME_WIDTH, FRAME_HEIGHT)
        )

        # Calculate grid position
        column = index % COLUMNS
        row = index // COLUMNS

        x = (
            OUTER_MARGIN
            + column * (FRAME_WIDTH + PADDING)
        )

        y = (
            OUTER_MARGIN
            + row * (FRAME_HEIGHT + PADDING)
        )

        # Place image
        sheet.paste(
            image,
            (x, y),
            image
        )

    # --------------------------------------------------------
    # Save
    # --------------------------------------------------------

    sheet.save(
        OUTPUT_FILE,
        format="PNG",
        optimize=False
    )

    print()
    print("========================================")
    print("Sprite sheet created!")
    print("========================================")
    print(f"Output: {OUTPUT_FILE}")
    print(f"Dimensions: {sheet_width} x {sheet_height}")
    print(f"Frame size: {FRAME_WIDTH} x {FRAME_HEIGHT}")
    print(f"Grid: {COLUMNS} x {ROWS}")
    print(f"Padding: {PADDING}px")
    print("========================================")


# ============================================================
# MAIN
# ============================================================

if __name__ == "__main__":
    create_sprite_sheet()