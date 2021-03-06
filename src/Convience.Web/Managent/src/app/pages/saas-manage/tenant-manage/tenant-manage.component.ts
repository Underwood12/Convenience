import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzMessageService, NzModalService } from 'ng-zorro-antd';
import { Tenant } from '../model/tenant';
import { TenantService } from 'src/app/business/tenant.service';

@Component({
  selector: 'app-tenant-manage',
  templateUrl: './tenant-manage.component.html',
  styleUrls: ['./tenant-manage.component.scss']
})
export class TenantManageComponent implements OnInit {

  sortkey: string = "CreatedTime";
  isdesc: boolean = true;

  editedTenant: Tenant = {};
  data: Tenant[] = [];

  searchForm: FormGroup = new FormGroup({});
  editForm: FormGroup = new FormGroup({});;

  tplModal: NzModalRef;

  page: number = 1;
  size: number = 10;
  total: number = 0;

  constructor(
    private _formBuilder: FormBuilder,
    private _tenantService: TenantService,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      name: [""],
      dataBaseType: [""]
    });
    this.refresh();
  }

  refresh() {
    this._tenantService.getTenants(this.page, this.size, this.searchForm.value['name'],
      this.searchForm.value['dataBaseType'], this.sortkey, this.isdesc).subscribe(result => {
        this.data = result['data'];
        this.total = result['count'];
      });
  }

  submitSearch() {
    this.refresh();
  }


  add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.editedTenant = new Tenant();
    this.editTenant(title, content, this.editedTenant);
  }

  edit(title: TemplateRef<{}>, content: TemplateRef<{}>, tenant: Tenant) {
    this._tenantService.getTenant(tenant.id).subscribe(result => {
      this.editedTenant = result;
      this.editTenant(title, content, this.editedTenant);
    })
  }

  private editTenant(title: TemplateRef<{}>, content: TemplateRef<{}>, tenant: Tenant) {
    this.editForm = this._formBuilder.group({
      name: [this.editedTenant.name, [Validators.required, Validators.maxLength(15),Validators.pattern('^[a-zA-Z0-9]*$')]],
      urlPrefix: [this.editedTenant.urlPrefix, [Validators.required, Validators.maxLength(15),Validators.pattern('^[a-zA-Z0-9]*$')]],
      dataBaseType: [this.editedTenant.dataBaseType, [Validators.required]],
      connectionString: [this.editedTenant.connectionString, [Validators.required]],
      isActive: [this.editedTenant.isActive],
    });
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzMaskClosable: false
    });
  }

  submitEdit() {
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    if (this.editForm.valid) {
      this.editedTenant.name = this.editForm.value["name"];
      this.editedTenant.urlPrefix = this.editForm.value["urlPrefix"];
      this.editedTenant.dataBaseType = this.editForm.value["dataBaseType"];
      this.editedTenant.connectionString = this.editForm.value["connectionString"];
      this.editedTenant.isActive = this.editForm.value["isActive"];
      if (this.editedTenant.id) {
        this._tenantService.update(this.editedTenant).subscribe(result => {
          this._messageService.success("更新成功！");
          this.refresh();
          this.tplModal.close();
        });
      } else {
        this._tenantService.add(this.editedTenant).subscribe(result => {
          this._messageService.success("添加成功！");
          this.refresh();
          this.tplModal.close();
        });
      }
    }
  }

  cancelEdit() {
    this.tplModal.close();
  }

  remove(id) {
    this._modalService.confirm({
      nzTitle: '是否删除该租户?',
      nzContent: null,
      nzOnOk: () =>
        this._tenantService.delete(id).subscribe(result => {
          this.refresh();
          this._messageService.success("删除成功！");
        })
    });
  }

  pageChange() {
    this.refresh();
  }

  sizeChange() {
    this.page = 1;
    this.refresh();
  }

  sorkKeyChange() {
    this.isdesc = false;
  }

  radioClick(event) {
    this.isdesc = !this.isdesc;
    this.refresh();
  }
}
