import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { Header } from '../header/header';
import { Menu } from '../menu/menu';
import { Footer } from '../footer/footer';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, Header, Menu, Footer],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {}
