# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.require_version ">= 1.8.1"

Vagrant.configure("2") do |config|
    config.vm.box = "windows_2012_r2"

    config.vm.provider :virtualbox do |v, override|
        v.linked_clone = true
        v.cpus = 2
        v.memory = 2048
        v.customize ["modifyvm", :id, "--vram", 64]
        v.customize ["modifyvm", :id, "--clipboard", "bidirectional"]
        v.customize ["modifyvm", :id, "--usb", "on"]
        #v.customize ["modifyvm", :id, "--usbehci", "on"]
        # see usbfilter reference with VBoxManage usbfilter
        # list the host USB devices with VBoxManage list usbhost
        v.customize [
                "usbfilter", "add", "0",
                "--target", :id,
                "--name", "USB Serial Port",
                "--manufacturer", "FTDI",
                #"--product", "FT232R USB UART",
                #"--serialnumber", "A50285BI",
            ]
    end

    config.vm.provision "shell", path: "provision.ps1"
end
