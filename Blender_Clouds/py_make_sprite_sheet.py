from PIL import Image
from pathlib import Path


# Settings
INPUT_FOLDER = Path("blender_render")
#INPUT_FOLDER = Path("images")
OUTPUT_FILE = Path("sprite_sheet.png")

GRID_SIZE = 8  # 8 x 8 = 64 images


def create_sprite_sheet():
    # Get images in filename order
    images = sorted(
        [
            p for p in INPUT_FOLDER.iterdir()
            if p.suffix.lower() in [".png", ".jpg", ".jpeg", ".bmp", ".webp"]
        ]
    )

    if len(images) != 64:
        raise ValueError(f"Expected 64 images, but found {len(images)}")

    # Open first image to determine dimensions
    first_image = Image.open(images[0]).convert("RGBA")
    width, height = first_image.size

    # Create 8x8 sprite sheet
    sheet = Image.new(
        "RGBA",
        (width * GRID_SIZE, height * GRID_SIZE),
        (0, 0, 0, 0)
    )

    # Place images in order
    for index, image_path in enumerate(images):
        image = Image.open(image_path).convert("RGBA")

        # Make sure all images have the same size
        if image.size != (width, height):
            raise ValueError(
                f"{image_path.name} has size {image.size}, "
                f"expected {(width, height)}"
            )

        row = index // GRID_SIZE
        column = index % GRID_SIZE

        x = column * width
        y = row * height

        sheet.paste(image, (x, y))

    # Save sprite sheet
    sheet.save(OUTPUT_FILE)

    print(f"Created: {OUTPUT_FILE}")
    print(f"Size: {sheet.size[0]}x{sheet.size[1]}")


if __name__ == "__main__":
    create_sprite_sheet()