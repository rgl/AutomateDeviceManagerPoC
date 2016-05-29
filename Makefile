screenshots:
	cp bin/Debug/*.png .
	find . -name '*.png' -exec gm convert {} -geometry x440 {} \;
	find . -name '*.png' -exec pngquant --ext .png --force {} \;

.PHONY: screenshots
