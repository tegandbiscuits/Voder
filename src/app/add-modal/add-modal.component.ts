import { Component, EventEmitter, HostListener, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-modal',
  templateUrl: './add-modal.component.html',
  styleUrls: ['./add-modal.component.css'],
})
export class AddModalComponent {
  @Output() modalClose = new EventEmitter<void>();

  feedSettings = new FormGroup({
    feedUrl: new FormControl(''),
  });

  @HostListener('window:keyup', ['$event'])
  keyEvent(event: KeyboardEvent) {
    if (event.key === 'Escape') {
      this.modalClose.emit();
    }
  }

  onSubmit() {
    console.log(this.feedSettings.value);
  }
}
