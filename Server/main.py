import argparse
import StringToObject
import run_rvo

def main():
    parser = argparse.ArgumentParser(description="Calculate square velocity.")
    parser.add_argument("json_str", type=str, help="object data")
    args = parser.parse_args()
    scene_data = StringToObject.StringToObject(args.json_str)
    run_rvo.run_rvo(scene_data)


if __name__ == "__main__":
    main()